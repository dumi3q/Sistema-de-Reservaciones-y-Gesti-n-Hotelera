using AutoMapper;
using Microsoft.Extensions.Logging;
using SistemaHotelero.Dominio.Clases;
using SistemaHotelero.Dominio.Entidades;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.Enumeraciones;
using SistemaHotelero.Dominio.InterfazAD;
using SistemaHotelero.Utilitarios;
using System.Globalization;

namespace SistemaHotelero.LogicaNegocio.Implementacion
{
    // Reglas compartidas por disponibilidad, reservas y estadías:
    // tarifas por noche, cruces de fechas, configuración y saldo de la cuenta.
    public abstract class BaseHospedajeLN<T> : BaseLN<T>
    {
        protected const int MaximoNoches = 30;
        protected const int MaximoHabitacionesPorReserva = 10;

        protected BaseHospedajeLN(IUnidadTrabajoEF unidadTrabajo, IMapper mapper, ILogger<T> logger)
            : base(unidadTrabajo, mapper, logger)
        {
        }

        protected static decimal Redondear(decimal valor)
            => Math.Round(valor, 2, MidpointRounding.AwayFromZero);

        protected static string NombreHabitacion(Habitacione habitacion)
            => $"{habitacion.Edificio?.Codigo}-{habitacion.Numero}";

        protected static string NombreCliente(Cliente? cliente)
            => cliente?.Persona == null ? string.Empty : $"{cliente.Persona.Nombres} {cliente.Persona.Apellidos}";

        #region Configuración

        protected async Task<Respuesta<Dictionary<string, string>>> CargarConfiguracionAsync()
        {
            var respuesta = await _unidadDeTrabajo.TConfiguracion.ListarAsync();
            if (!respuesta.Success)
                return Falla<Dictionary<string, string>, IEnumerable<Configuracion>>(respuesta);

            return Respuesta<Dictionary<string, string>>.Ok(
                respuesta.Data!.ToDictionary(x => x.Clave, x => x.Valor, StringComparer.OrdinalIgnoreCase));
        }

        protected static decimal ConfigDecimal(Dictionary<string, string> config, string clave, decimal porDefecto)
            => config.TryGetValue(clave, out var valor)
               && decimal.TryParse(valor, NumberStyles.Number, CultureInfo.InvariantCulture, out var numero)
                ? numero
                : porDefecto;

        protected static TimeOnly ConfigHora(Dictionary<string, string> config, string clave, TimeOnly porDefecto)
            => config.TryGetValue(clave, out var valor)
               && TimeOnly.TryParseExact(valor, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var hora)
                ? hora
                : porDefecto;

        protected static string ConfigTexto(Dictionary<string, string> config, string clave, string porDefecto)
            => config.TryGetValue(clave, out var valor) && !string.IsNullOrWhiteSpace(valor) ? valor.Trim() : porDefecto;

        // IVA de hospedaje vigente en la fecha. Si no hay ninguno configurado, se cobra 0 %.
        protected async Task<Respuesta<Impuesto?>> ObtenerImpuestoHospedajeAsync(DateOnly fecha)
        {
            var ambito = (byte)AmbitoImpuesto.Hospedaje;
            var respuesta = await _unidadDeTrabajo.TImpuesto.BuscarAsync(
                x => x.Activo && x.Ambito == ambito
                  && x.VigenteDesde <= fecha
                  && (x.VigenteHasta == null || x.VigenteHasta >= fecha));

            if (!respuesta.Success)
                return Falla<Impuesto?, IEnumerable<Impuesto>>(respuesta);

            return Respuesta<Impuesto?>.Ok(respuesta.Data!.OrderByDescending(x => x.VigenteDesde).FirstOrDefault());
        }

        #endregion

        #region Fechas y disponibilidad

        protected static string? ValidarFechas(DateOnly inicio, DateOnly fin)
        {
            if (inicio == default || fin == default)
                return "Indique la fecha de entrada y la de salida.";

            if (inicio >= fin)
                return "La fecha de salida debe ser posterior a la de entrada.";

            if (inicio < HoyHotel())
                return "La fecha de entrada no puede ser anterior a hoy.";

            if (fin.DayNumber - inicio.DayNumber > MaximoNoches)
                return $"Una reserva puede tener como máximo {MaximoNoches} noches.";

            return null;
        }

        // Habitaciones con alguna noche reservada o bloqueada dentro de [inicio, fin)
        protected async Task<Respuesta<HashSet<int>>> HabitacionesNoLibresAsync(DateOnly inicio, DateOnly fin)
        {
            var noches = await _unidadDeTrabajo.TReservacionNoche.BuscarAsync(
                x => x.Activa && x.Fecha >= inicio && x.Fecha < fin);
            if (!noches.Success)
                return Falla<HashSet<int>, IEnumerable<ReservacionNoch>>(noches);

            var bloqueos = await _unidadDeTrabajo.TBloqueoHabitacion.BuscarAsync(
                x => x.Activo && x.FechaInicio < fin && inicio < x.FechaFin);
            if (!bloqueos.Success)
                return Falla<HashSet<int>, IEnumerable<BloqueosHabitacion>>(bloqueos);

            var ids = noches.Data!.Select(x => x.HabitacionId)
                .Concat(bloqueos.Data!.Select(x => x.HabitacionId))
                .ToHashSet();

            return Respuesta<HashSet<int>>.Ok(ids);
        }

        // El estado operativo de hoy solo importa si la estadía empieza hoy
        protected static bool EstadoPermiteReservar(Habitacione habitacion, DateOnly inicio)
        {
            var estado = (EstadoHabitacion)habitacion.Estado;

            if (estado == EstadoHabitacion.FueraDeServicio)
                return false;

            if (inicio == HoyHotel() && estado is EstadoHabitacion.EnMantenimiento or EstadoHabitacion.Bloqueada)
                return false;

            return true;
        }

        protected static bool CabenHuespedes(TiposHabitacion tipo, int adultos, int ninos)
            => adultos >= 1
               && adultos <= tipo.CapacidadAdultos
               && ninos <= tipo.CapacidadNinos
               && adultos + ninos <= tipo.CapacidadMaxima;

        #endregion

        #region Tarifas

        protected async Task<Respuesta<List<ReglasTarifa>>> CargarReglasAsync()
        {
            var respuesta = await _unidadDeTrabajo.TReglaTarifa.BuscarAsync(x => x.Activo);
            if (!respuesta.Success)
                return Falla<List<ReglasTarifa>, IEnumerable<ReglasTarifa>>(respuesta);

            return Respuesta<List<ReglasTarifa>>.Ok(respuesta.Data!.ToList());
        }

        /* Precio de cada noche de [inicio, fin):
           base = precio del tipo + precio por persona adicional sobre la ocupación base.
           Si varias reglas aplican a la misma noche, gana la de mayor Prioridad;
           en empate, la específica del tipo sobre la general, y luego la de mayor valor. */
        protected static List<TReservacionNoche> CalcularNoches(
            TiposHabitacion tipo, int personas, DateOnly inicio, DateOnly fin, List<ReglasTarifa> reglas)
        {
            var adicionales = Math.Max(0, personas - tipo.OcupacionBase);
            var tarifaBase = Redondear(tipo.PrecioBasePorNoche + tipo.PrecioPersonaAdicional * adicionales);

            var noches = new List<TReservacionNoche>();

            for (var fecha = inicio; fecha < fin; fecha = fecha.AddDays(1))
            {
                var diaIso = (byte)(((int)fecha.DayOfWeek + 6) % 7 + 1); // lunes = 1 ... domingo = 7

                var regla = reglas
                    .Where(r => r.TipoHabitacionId == null || r.TipoHabitacionId == tipo.Id)
                    .Where(r => (r.Tipo == (byte)TipoReglaTarifa.Temporada
                                    && r.FechaInicio <= fecha && fecha <= r.FechaFin)
                             || (r.Tipo == (byte)TipoReglaTarifa.FinDeSemana && r.DiaSemana == diaIso))
                    .OrderByDescending(r => r.Prioridad)
                    .ThenByDescending(r => r.TipoHabitacionId != null)
                    .ThenByDescending(r => r.Valor)
                    .FirstOrDefault();

                decimal ajuste = 0;
                if (regla != null)
                {
                    ajuste = regla.TipoAjuste == (byte)TipoAjusteTarifa.Porcentaje
                        ? Redondear(tarifaBase * regla.Valor / 100m)
                        : regla.Valor;
                }

                noches.Add(new TReservacionNoche
                {
                    Fecha = fecha,
                    TarifaBase = tarifaBase,
                    AjusteTarifa = ajuste,
                    TarifaFinal = Math.Max(0, tarifaBase + ajuste),
                    ReglaAplicada = regla?.Nombre
                });
            }

            return noches;
        }

        /* Cotización completa en el servidor. La usan api/Reservacion/Cotizar y la creación
           de la reserva, así el precio guardado es siempre el mismo que se mostró. */
        protected async Task<Respuesta<TCotizacion>> CotizarInternoAsync(clsSolicitudReserva solicitud)
        {
            var errorFechas = ValidarFechas(solicitud.FechaInicio, solicitud.FechaFin);
            if (errorFechas != null)
                return Falla<TCotizacion>(errorFechas);

            solicitud.Habitaciones ??= new();

            if (solicitud.Habitaciones.Count == 0)
                return Falla<TCotizacion>("Indique al menos una habitación.");

            if (solicitud.Habitaciones.Count > MaximoHabitacionesPorReserva)
                return Falla<TCotizacion>($"Una reserva puede tener como máximo {MaximoHabitacionesPorReserva} habitaciones.");

            if (solicitud.Habitaciones.GroupBy(x => x.HabitacionId).Any(g => g.Count() > 1))
                return Falla<TCotizacion>("Hay habitaciones repetidas en la solicitud.");

            if (solicitud.Habitaciones.Any(x => x.Adultos < 1))
                return Falla<TCotizacion>("Cada habitación debe tener al menos un adulto.");

            foreach (var solicitada in solicitud.Habitaciones)
            {
                var errorHuespedes = ValidarHuespedes(solicitada);
                if (errorHuespedes != null)
                    return Falla<TCotizacion>(errorHuespedes);
            }

            var cliente = await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(x => x.Id == solicitud.ClienteId);
            if (!cliente.Success)
                return Falla<TCotizacion, Cliente>(cliente);
            if (cliente.Data == null)
                return Falla<TCotizacion>("Cliente no encontrado.", CodigosRespuesta.NoEncontrado);
            if (!cliente.Data.Activo)
                return Falla<TCotizacion>("El cliente está inactivo.");

            var ids = solicitud.Habitaciones.Select(x => x.HabitacionId).ToList();
            var habitaciones = await _unidadDeTrabajo.THabitacion.BuscarAsync(
                x => ids.Contains(x.Id), new List<string> { "Edificio", "TipoHabitacion" });
            if (!habitaciones.Success)
                return Falla<TCotizacion, IEnumerable<Habitacione>>(habitaciones);

            var porId = habitaciones.Data!.ToDictionary(x => x.Id);

            var noLibres = await HabitacionesNoLibresAsync(solicitud.FechaInicio, solicitud.FechaFin);
            if (!noLibres.Success)
                return Falla<TCotizacion, HashSet<int>>(noLibres);

            var reglas = await CargarReglasAsync();
            if (!reglas.Success)
                return Falla<TCotizacion, List<ReglasTarifa>>(reglas);

            var impuesto = await ObtenerImpuestoHospedajeAsync(solicitud.FechaInicio);
            if (!impuesto.Success)
                return Falla<TCotizacion, Impuesto?>(impuesto);

            var config = await CargarConfiguracionAsync();
            if (!config.Success)
                return Falla<TCotizacion, Dictionary<string, string>>(config);

            var porcentajeDescuento = cliente.Data.PorcentajeDescuento;
            var porcentajeImpuesto = impuesto.Data?.Porcentaje ?? 0m;

            var cotizacion = new TCotizacion
            {
                ClienteId = cliente.Data.Id,
                FechaInicio = solicitud.FechaInicio,
                FechaFin = solicitud.FechaFin,
                Noches = solicitud.FechaFin.DayNumber - solicitud.FechaInicio.DayNumber,
                PorcentajeDescuentoCliente = porcentajeDescuento,
                PorcentajeImpuesto = porcentajeImpuesto
            };

            decimal anticipoPrimeraNoche = 0;

            foreach (var solicitada in solicitud.Habitaciones)
            {
                if (!porId.TryGetValue(solicitada.HabitacionId, out var habitacion))
                    return Falla<TCotizacion>($"La habitación {solicitada.HabitacionId} no existe.", CodigosRespuesta.NoEncontrado);

                var nombre = NombreHabitacion(habitacion);

                if (!habitacion.Edificio.Activo || !habitacion.TipoHabitacion.Activo || !EstadoPermiteReservar(habitacion, solicitud.FechaInicio))
                    return Falla<TCotizacion>($"La habitación {nombre} no se puede reservar en este momento.", CodigosRespuesta.Conflicto);

                if (!CabenHuespedes(habitacion.TipoHabitacion, solicitada.Adultos, solicitada.Ninos))
                    return Falla<TCotizacion>(
                        $"La habitación {nombre} ({habitacion.TipoHabitacion.Nombre}) admite máximo "
                        + $"{habitacion.TipoHabitacion.CapacidadAdultos} adultos, {habitacion.TipoHabitacion.CapacidadNinos} niños "
                        + $"y {habitacion.TipoHabitacion.CapacidadMaxima} personas en total.");

                if (noLibres.Data!.Contains(habitacion.Id))
                    return Falla<TCotizacion>($"La habitación {nombre} no está disponible en esas fechas.", CodigosRespuesta.Conflicto);

                var noches = CalcularNoches(
                    habitacion.TipoHabitacion, solicitada.Adultos + solicitada.Ninos,
                    solicitud.FechaInicio, solicitud.FechaFin, reglas.Data!);

                var subtotal = noches.Sum(x => x.TarifaFinal);
                var descuento = Redondear(subtotal * porcentajeDescuento / 100m);
                var impuestos = Redondear((subtotal - descuento) * porcentajeImpuesto / 100m);

                cotizacion.Habitaciones.Add(new TReservacionHabitacion
                {
                    HabitacionId = habitacion.Id,
                    Habitacion = nombre,
                    TipoHabitacionNombre = habitacion.TipoHabitacion.Nombre,
                    FechaInicio = solicitud.FechaInicio,
                    FechaFin = solicitud.FechaFin,
                    Adultos = solicitada.Adultos,
                    Ninos = solicitada.Ninos,
                    Subtotal = subtotal,
                    Descuento = descuento,
                    Impuestos = impuestos,
                    Total = subtotal - descuento + impuestos,
                    Noches = noches
                });

                var primera = noches[0].TarifaFinal * (1 - porcentajeDescuento / 100m);
                anticipoPrimeraNoche += primera * (1 + porcentajeImpuesto / 100m);
            }

            cotizacion.Subtotal = cotizacion.Habitaciones.Sum(x => x.Subtotal);
            cotizacion.Descuento = cotizacion.Habitaciones.Sum(x => x.Descuento);
            cotizacion.Impuestos = cotizacion.Habitaciones.Sum(x => x.Impuestos);
            cotizacion.Total = cotizacion.Habitaciones.Sum(x => x.Total);

            // AnticipoTipo: "Porcentaje" (AnticipoValor % del total) o "PrimeraNoche"
            var tipoAnticipo = ConfigTexto(config.Data!, "AnticipoTipo", "Porcentaje");
            var anticipo = tipoAnticipo.Equals("PrimeraNoche", StringComparison.OrdinalIgnoreCase)
                ? Redondear(anticipoPrimeraNoche)
                : Redondear(cotizacion.Total * ConfigDecimal(config.Data!, "AnticipoValor", 30m) / 100m);

            cotizacion.AnticipoRequerido = Math.Min(anticipo, cotizacion.Total);

            return Respuesta<TCotizacion>.Ok(cotizacion);
        }

        private static string? ValidarHuespedes(clsHabitacionSolicitada solicitada)
        {
            solicitada.Huespedes ??= new();

            if (solicitada.Huespedes.Count > solicitada.Adultos + solicitada.Ninos)
                return "Hay más huéspedes que personas indicadas para la habitación.";

            if (solicitada.Huespedes.Count(x => x.EsTitular) > 1)
                return "Solo puede haber un huésped titular por habitación.";

            foreach (var huesped in solicitada.Huespedes)
            {
                if (string.IsNullOrWhiteSpace(huesped.Nombres) || huesped.Nombres.Trim().Length > 100
                    || string.IsNullOrWhiteSpace(huesped.Apellidos) || huesped.Apellidos.Trim().Length > 100)
                    return "Cada huésped necesita nombres y apellidos (máximo 100 caracteres).";

                var sinNumero = string.IsNullOrWhiteSpace(huesped.NumeroDocumento);
                if ((huesped.TipoDocumentoId == null) != sinNumero)
                    return "Indique el tipo y el número de documento del huésped, o ninguno de los dos.";
            }

            return null;
        }

        #endregion

        #region Cuenta: pagado y saldo

        // Pagado neto = pagos menos lo devuelto en reversos (un pago revertido completo suma 0)
        protected async Task<Respuesta<decimal>> CalcularPagadoAsync(int cuentaId)
        {
            var pagos = await _unidadDeTrabajo.TPago.BuscarAsync(
                x => x.CuentaId == cuentaId, new List<string> { "ReversosPagos" });
            if (!pagos.Success)
                return Falla<decimal, IEnumerable<Pago>>(pagos);

            return Respuesta<decimal>.Ok(pagos.Data!.Sum(p => p.Monto - p.ReversosPagos.Sum(r => r.Monto)));
        }

        // Saldo = cargos - pagado neto. Positivo: el cliente debe; negativo: saldo a favor.
        protected async Task<Respuesta<decimal>> CalcularSaldoAsync(int cuentaId)
        {
            var cargos = await _unidadDeTrabajo.TCargoCuenta.BuscarAsync(x => x.CuentaId == cuentaId);
            if (!cargos.Success)
                return Falla<decimal, IEnumerable<CargosCuentum>>(cargos);

            var pagado = await CalcularPagadoAsync(cuentaId);
            if (!pagado.Success)
                return pagado;

            return Respuesta<decimal>.Ok(cargos.Data!.Sum(x => x.Total) - pagado.Data);
        }

        #endregion

        #region Historial

        protected async Task<Respuesta<bool>> RegistrarCambioHabitacionAsync(
            Habitacione habitacion, EstadoHabitacion nuevo, int usuarioId, string motivo)
        {
            var anterior = habitacion.Estado;
            habitacion.Estado = (byte)nuevo;

            var historial = await _unidadDeTrabajo.THistorialEstadoHabitacion.InsertarAsync(
                new HistorialEstadosHabitacion
                {
                    HabitacionId = habitacion.Id,
                    UsuarioId = usuarioId,
                    EstadoAnterior = anterior,
                    EstadoNuevo = (byte)nuevo,
                    FechaUtc = DateTime.UtcNow,
                    Motivo = motivo
                });

            return historial.Success ? Respuesta<bool>.Ok(true) : Falla<bool, HistorialEstadosHabitacion>(historial);
        }

        protected async Task<Respuesta<bool>> RegistrarCambioReservaAsync(
            Reservacione reservacion, EstadoReservacion nuevo, int usuarioId, string? motivo, string? detalleCambio = null)
        {
            var anterior = reservacion.Estado;
            reservacion.Estado = (byte)nuevo;

            var historial = await _unidadDeTrabajo.THistorialReservacion.InsertarAsync(
                new HistorialReservacione
                {
                    ReservacionId = reservacion.Id,
                    UsuarioId = usuarioId,
                    FechaUtc = DateTime.UtcNow,
                    EstadoAnterior = anterior,
                    EstadoNuevo = (byte)nuevo,
                    Motivo = motivo,
                    DetalleCambio = detalleCambio
                });

            return historial.Success ? Respuesta<bool>.Ok(true) : Falla<bool, HistorialReservacione>(historial);
        }

        #endregion
    }
}
