using AutoMapper;
using Microsoft.Extensions.Logging;
using SistemaHotelero.Dominio.Entidades;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.Enumeraciones;
using SistemaHotelero.Dominio.InterfazAD;
using SistemaHotelero.Dominio.InterfazLN;
using SistemaHotelero.Utilitarios;
using System.Security.Cryptography;
using System.Text;

namespace SistemaHotelero.LogicaNegocio.Implementacion
{
    public class EstadiaLN : BaseHospedajeLN<EstadiaLN>, IEstadiaLN
    {
        private static readonly List<string> IncludesEstadia = new()
        {
            "ReservacionHabitacion.Habitacion.Edificio",
            "ReservacionHabitacion.Reservacion.Cliente.Persona",
            "ReservacionHabitacion.Reservacion.Cuenta"
        };

        public EstadiaLN(
            IUnidadTrabajoEF unidadTrabajo,
            ILogger<EstadiaLN> logger,
            IMapper mapper)
            : base(unidadTrabajo, mapper, logger)
        {
        }

        #region Check-in

        /* Check-in de todas las habitaciones de la reserva:
           crea la estadía y su código de acceso, carga el hospedaje a la cuenta,
           pasa cada habitación a Ocupada y la reserva a EnCurso. Todo en una transacción. */
        public async Task<Respuesta<TCheckIn>> CheckInAsync(int reservacionId, int usuarioId)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.TReservacion.ObtenerEntidadAsync(
                    x => x.Id == reservacionId,
                    new List<string>
                    {
                        "Cuenta",
                        "ReservacionHabitaciones.Habitacion.Edificio",
                        "ReservacionHabitaciones.Estadia"
                    });

                if (!respuesta.Success)
                    return Falla<TCheckIn, Reservacione>(respuesta);
                if (respuesta.Data == null)
                    return Falla<TCheckIn>("Reservación no encontrada.", CodigosRespuesta.NoEncontrado);

                var reservacion = respuesta.Data;
                var estado = (EstadoReservacion)reservacion.Estado;

                if (estado == EstadoReservacion.PendientePago)
                    return Falla<TCheckIn>("La reserva está pendiente de pago: primero hay que confirmarla con el anticipo.", CodigosRespuesta.Conflicto);

                if (estado != EstadoReservacion.Confirmada)
                    return Falla<TCheckIn>($"No se puede hacer check-in de una reserva en estado {estado}.", CodigosRespuesta.Conflicto);

                var hoy = HoyHotel();
                if (hoy < reservacion.FechaInicio)
                    return Falla<TCheckIn>($"La reserva empieza el {reservacion.FechaInicio:dd/MM/yyyy}.", CodigosRespuesta.Conflicto);

                if (hoy >= reservacion.FechaFin)
                    return Falla<TCheckIn>("La fecha de salida de la reserva ya pasó.", CodigosRespuesta.Conflicto);

                if (reservacion.Cuenta == null)
                    return Falla<TCheckIn>("La reservación no tiene cuenta de hospedaje.", CodigosRespuesta.Error);

                if (reservacion.ReservacionHabitaciones.Any(x => x.Estadia != null))
                    return Falla<TCheckIn>("La reserva ya tiene check-in.", CodigosRespuesta.Conflicto);

                // Solo se entrega una habitación lista (no en limpieza, mantenimiento, etc.)
                var noListas = reservacion.ReservacionHabitaciones
                    .Where(x => x.Habitacion.Estado != (byte)EstadoHabitacion.Disponible
                             && x.Habitacion.Estado != (byte)EstadoHabitacion.Reservada)
                    .Select(x => $"{NombreHabitacion(x.Habitacion)} ({(EstadoHabitacion)x.Habitacion.Estado})")
                    .ToList();

                if (noListas.Count > 0)
                    return Falla<TCheckIn>("Habitaciones que no están listas: " + string.Join(", ", noListas), CodigosRespuesta.Conflicto);

                var config = await CargarConfiguracionAsync();
                if (!config.Success)
                    return Falla<TCheckIn, Dictionary<string, string>>(config);

                var impuesto = await ObtenerImpuestoHospedajeAsync(reservacion.FechaInicio);
                if (!impuesto.Success)
                    return Falla<TCheckIn, Impuesto?>(impuesto);

                var horaSalida = ConfigHora(config.Data!, "HoraCheckOut", new TimeOnly(12, 0));
                var ahoraUtc = DateTime.UtcNow;

                var resultado = new TCheckIn { ReservacionId = reservacion.Id, Codigo = reservacion.Codigo };

                _unidadDeTrabajo.EmpezarTransaccion();

                foreach (var linea in reservacion.ReservacionHabitaciones.OrderBy(x => x.Id))
                {
                    var nombre = NombreHabitacion(linea.Habitacion);
                    var noches = linea.FechaFin.DayNumber - linea.FechaInicio.DayNumber;

                    // Código de 6 dígitos para la puerta. En la BD solo queda su hash SHA-256.
                    var codigoAcceso = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
                    var expira = LocalHotelAUtc(linea.FechaFin, horaSalida);

                    var estadia = new Estadia
                    {
                        ReservacionHabitacionId = linea.Id,
                        CheckInRealUtc = ahoraUtc,
                        UsuarioCheckInId = usuarioId,
                        Estado = (byte)EstadoEstadia.Activa
                    };

                    estadia.AccesosEstadia.Add(new AccesosEstadium
                    {
                        IdentificadorAcceso = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(codigoAcceso))),
                        Tipo = (byte)TipoAccesoEstadia.CodigoDigital,
                        FechaEmisionUtc = ahoraUtc,
                        FechaExpiracionUtc = expira
                    });

                    // El hospedaje se carga a la cuenta al entrar: así el saldo refleja lo que se debe
                    estadia.CargosCuenta.Add(new CargosCuentum
                    {
                        CuentaId = reservacion.Cuenta.Id,
                        Tipo = (byte)TipoCargo.Hospedaje,
                        Concepto = $"Hospedaje {nombre} ({noches} noche{(noches == 1 ? "" : "s")})",
                        Cantidad = 1,
                        PrecioUnitario = linea.Subtotal,
                        Descuento = linea.Descuento,
                        ImpuestoId = impuesto.Data?.Id,
                        Impuestos = linea.Impuestos,
                        Total = linea.Total,
                        FechaUtc = ahoraUtc,
                        UsuarioId = usuarioId
                    });

                    var creada = await _unidadDeTrabajo.TEstadia.InsertarAsync(estadia);
                    if (!creada.Success)
                    {
                        _unidadDeTrabajo.Rollback();
                        return Falla<TCheckIn, Estadia>(creada);
                    }

                    var cambio = await RegistrarCambioHabitacionAsync(
                        linea.Habitacion, EstadoHabitacion.Ocupada, usuarioId, $"Check-in {reservacion.Codigo}");
                    if (!cambio.Success)
                    {
                        _unidadDeTrabajo.Rollback();
                        return Falla<TCheckIn, bool>(cambio);
                    }

                    resultado.Accesos.Add(new TAccesoEmitido
                    {
                        EstadiaId = estadia.Id,
                        Habitacion = nombre,
                        CodigoAcceso = codigoAcceso,
                        ExpiraUtc = expira
                    });
                }

                var cambioReserva = await RegistrarCambioReservaAsync(
                    reservacion, EstadoReservacion.EnCurso, usuarioId, "Check-in");
                if (!cambioReserva.Success)
                {
                    _unidadDeTrabajo.Rollback();
                    return Falla<TCheckIn, bool>(cambioReserva);
                }

                _unidadDeTrabajo.CompletarTran();

                return Respuesta<TCheckIn>.Ok(resultado);
            }
            catch (Exception ex)
            {
                _unidadDeTrabajo.Rollback();
                return Excepcion<TCheckIn>(ex, $"Error en el check-in de la reservación {reservacionId}.");
            }
        }

        #endregion

        #region Check-out

        /* Check-out de toda la reserva. Solo con saldo cero.
           Si sale después de la hora de check-out, primero se carga el recargo por hora
           y se pide pagarlo antes de volver a intentar. */
        public async Task<Respuesta<TCheckOut>> CheckOutAsync(int reservacionId, int usuarioId)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.TReservacion.ObtenerEntidadAsync(
                    x => x.Id == reservacionId,
                    new List<string>
                    {
                        "Cuenta",
                        "ReservacionHabitaciones.Habitacion.Edificio",
                        "ReservacionHabitaciones.Estadia.AccesosEstadia",
                        "ReservacionHabitaciones.ReservacionNoches"
                    });

                if (!respuesta.Success)
                    return Falla<TCheckOut, Reservacione>(respuesta);
                if (respuesta.Data == null)
                    return Falla<TCheckOut>("Reservación no encontrada.", CodigosRespuesta.NoEncontrado);

                var reservacion = respuesta.Data;

                if (reservacion.Estado != (byte)EstadoReservacion.EnCurso)
                    return Falla<TCheckOut>(
                        $"Solo se hace check-out de reservas en curso (estado actual: {(EstadoReservacion)reservacion.Estado}).",
                        CodigosRespuesta.Conflicto);

                if (reservacion.Cuenta == null)
                    return Falla<TCheckOut>("La reservación no tiene cuenta de hospedaje.", CodigosRespuesta.Error);

                var lineasActivas = reservacion.ReservacionHabitaciones
                    .Where(x => x.Estadia != null && x.Estadia.Estado == (byte)EstadoEstadia.Activa)
                    .ToList();

                if (lineasActivas.Count == 0)
                    return Falla<TCheckOut>("La reserva no tiene estadías activas.", CodigosRespuesta.Conflicto);

                var config = await CargarConfiguracionAsync();
                if (!config.Success)
                    return Falla<TCheckOut, Dictionary<string, string>>(config);

                // Salida tardía
                var recargo = await AplicarLateCheckOutAsync(reservacion, lineasActivas[0].Estadia!, config.Data!, usuarioId);
                if (!recargo.Success)
                    return Falla<TCheckOut, decimal>(recargo);

                var saldo = await CalcularSaldoAsync(reservacion.Cuenta.Id);
                if (!saldo.Success)
                    return Falla<TCheckOut, decimal>(saldo);

                if (saldo.Data != 0)
                {
                    var detalle = recargo.Data > 0
                        ? $"Se cargó un recargo por salida tardía de {recargo.Data:0.00}. "
                        : string.Empty;

                    var mensaje = saldo.Data > 0
                        ? $"{detalle}Saldo pendiente: {saldo.Data:0.00}. Registre el pago en la cuenta {reservacion.Cuenta.Id} y repita el check-out."
                        : $"La cuenta {reservacion.Cuenta.Id} tiene saldo a favor de {-saldo.Data:0.00}. Hay que devolverlo antes del check-out.";

                    return Falla<TCheckOut>(mensaje, CodigosRespuesta.Conflicto);
                }

                var hoy = HoyHotel();
                var resultado = new TCheckOut
                {
                    ReservacionId = reservacion.Id,
                    Codigo = reservacion.Codigo,
                    CuentaId = reservacion.Cuenta.Id
                };

                _unidadDeTrabajo.EmpezarTransaccion();

                foreach (var linea in lineasActivas)
                {
                    var estadia = linea.Estadia!;

                    // CHECK de la tabla: la salida debe ser posterior a la entrada (precisión de segundos)
                    var salida = DateTime.UtcNow;
                    if (salida <= estadia.CheckInRealUtc.AddSeconds(1))
                        salida = estadia.CheckInRealUtc.AddSeconds(1);

                    estadia.Estado = (byte)EstadoEstadia.Finalizada;
                    estadia.CheckOutRealUtc = salida;
                    estadia.UsuarioCheckOutId = usuarioId;
                    resultado.CheckOutUtc = salida;

                    foreach (var acceso in estadia.AccesosEstadia.Where(x => x.FechaRevocacionUtc == null))
                        acceso.FechaRevocacionUtc = salida;

                    // Salida anticipada: las noches que no se usarán quedan libres para otros
                    foreach (var noche in linea.ReservacionNoches.Where(x => x.Activa && x.Fecha >= hoy))
                        noche.Activa = false;

                    var cambio = await RegistrarCambioHabitacionAsync(
                        linea.Habitacion, EstadoHabitacion.EnLimpieza, usuarioId, $"Check-out {reservacion.Codigo}");
                    if (!cambio.Success)
                    {
                        _unidadDeTrabajo.Rollback();
                        return Falla<TCheckOut, bool>(cambio);
                    }

                    resultado.HabitacionesEnLimpieza.Add(NombreHabitacion(linea.Habitacion));
                }

                var cambioReserva = await RegistrarCambioReservaAsync(
                    reservacion, EstadoReservacion.Finalizada, usuarioId, "Check-out");
                if (!cambioReserva.Success)
                {
                    _unidadDeTrabajo.Rollback();
                    return Falla<TCheckOut, bool>(cambioReserva);
                }

                _unidadDeTrabajo.CompletarTran();

                return Respuesta<TCheckOut>.Ok(resultado);
            }
            catch (Exception ex)
            {
                _unidadDeTrabajo.Rollback();
                return Excepcion<TCheckOut>(ex, $"Error en el check-out de la reservación {reservacionId}.");
            }
        }

        // Devuelve el monto cargado ahora (0 si no correspondía o si ya se había cargado)
        private async Task<Respuesta<decimal>> AplicarLateCheckOutAsync(
            Reservacione reservacion, Estadia estadia, Dictionary<string, string> config, int usuarioId)
        {
            var horaSalida = ConfigHora(config, "HoraCheckOut", new TimeOnly(12, 0));
            var precioHora = ConfigDecimal(config, "RecargoLateCheckOutPorHora", 0m);

            var limite = reservacion.FechaFin.ToDateTime(horaSalida);
            var ahora = AhoraHotel();

            if (ahora <= limite || precioHora <= 0)
                return Respuesta<decimal>.Ok(0);

            var tipoLate = (byte)TipoCargo.LateCheckOut;
            var cuentaId = reservacion.Cuenta!.Id;

            var yaCargado = await _unidadDeTrabajo.TCargoCuenta.ContarAsync(x => x.CuentaId == cuentaId && x.Tipo == tipoLate);
            if (!yaCargado.Success)
                return Falla<decimal, int?>(yaCargado);
            if (yaCargado.Data > 0)
                return Respuesta<decimal>.Ok(0);

            var horas = (int)Math.Ceiling((ahora - limite).TotalHours);

            var impuesto = await ObtenerImpuestoHospedajeAsync(HoyHotel());
            if (!impuesto.Success)
                return Falla<decimal, Impuesto?>(impuesto);

            var subtotal = Redondear(horas * precioHora);
            var impuestos = Redondear(subtotal * (impuesto.Data?.Porcentaje ?? 0m) / 100m);

            var cargo = await _unidadDeTrabajo.TCargoCuenta.InsertarAsync(new CargosCuentum
            {
                CuentaId = cuentaId,
                Tipo = tipoLate,
                EstadiaId = estadia.Id,
                Concepto = $"Salida tardía ({horas} h)",
                Cantidad = horas,
                PrecioUnitario = precioHora,
                Descuento = 0,
                ImpuestoId = impuesto.Data?.Id,
                Impuestos = impuestos,
                Total = subtotal + impuestos,
                FechaUtc = DateTime.UtcNow,
                UsuarioId = usuarioId
            });

            return cargo.Success
                ? Respuesta<decimal>.Ok(subtotal + impuestos)
                : Falla<decimal, CargosCuentum>(cargo);
        }

        #endregion

        #region Consultas

        public async Task<Respuesta<IEnumerable<TEstadia>>> ListarActivasAsync()
        {
            try
            {
                var activa = (byte)EstadoEstadia.Activa;
                var respuesta = await _unidadDeTrabajo.TEstadia.BuscarAsync(x => x.Estado == activa, IncludesEstadia);

                if (!respuesta.Success)
                    return Falla<IEnumerable<TEstadia>, IEnumerable<Estadia>>(respuesta);

                return Respuesta<IEnumerable<TEstadia>>.Ok(
                    respuesta.Data!.Select(Mapear).OrderBy(x => x.Habitacion));
            }
            catch (Exception ex)
            {
                return Excepcion<IEnumerable<TEstadia>>(ex, "Error al listar estadías activas.");
            }
        }

        public async Task<Respuesta<TEstadia>> ObtenerActivaPorHabitacionAsync(int habitacionId)
        {
            try
            {
                var activa = (byte)EstadoEstadia.Activa;
                var respuesta = await _unidadDeTrabajo.TEstadia.ObtenerEntidadAsync(
                    x => x.Estado == activa && x.ReservacionHabitacion.HabitacionId == habitacionId, IncludesEstadia);

                if (!respuesta.Success)
                    return Falla<TEstadia, Estadia>(respuesta);

                if (respuesta.Data == null)
                    return Falla<TEstadia>("La habitación no tiene una estadía activa.", CodigosRespuesta.NoEncontrado);

                return Respuesta<TEstadia>.Ok(Mapear(respuesta.Data));
            }
            catch (Exception ex)
            {
                return Excepcion<TEstadia>(ex, $"Error al buscar la estadía activa de la habitación {habitacionId}.");
            }
        }

        private static TEstadia Mapear(Estadia e)
        {
            var linea = e.ReservacionHabitacion;
            var reservacion = linea.Reservacion;

            return new TEstadia
            {
                Id = e.Id,
                ReservacionHabitacionId = linea.Id,
                ReservacionId = reservacion.Id,
                CodigoReserva = reservacion.Codigo,
                HabitacionId = linea.HabitacionId,
                Habitacion = NombreHabitacion(linea.Habitacion),
                ClienteId = reservacion.ClienteId,
                ClienteNombre = NombreCliente(reservacion.Cliente),
                CuentaId = reservacion.Cuenta?.Id,
                CheckInRealUtc = e.CheckInRealUtc,
                CheckOutRealUtc = e.CheckOutRealUtc,
                Estado = e.Estado
            };
        }

        #endregion
    }
}
