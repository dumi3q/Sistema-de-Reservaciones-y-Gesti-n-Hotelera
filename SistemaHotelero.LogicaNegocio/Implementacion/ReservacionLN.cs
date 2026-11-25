using AutoMapper;
using Microsoft.Extensions.Logging;
using SistemaHotelero.Dominio.Clases;
using SistemaHotelero.Dominio.Entidades;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.Enumeraciones;
using SistemaHotelero.Dominio.InterfazAD;
using SistemaHotelero.Dominio.InterfazLN;
using SistemaHotelero.Utilitarios;

namespace SistemaHotelero.LogicaNegocio.Implementacion
{
    public class ReservacionLN : BaseHospedajeLN<ReservacionLN>, IReservacionLN
    {
        private static readonly List<string> IncludesDetalle = new()
        {
            "Cliente.Persona",
            "Cuenta",
            "ReservacionHabitaciones.Habitacion.Edificio",
            "ReservacionHabitaciones.Habitacion.TipoHabitacion",
            "ReservacionHabitaciones.ReservacionNoches",
            "ReservacionHabitaciones.HuespedesReservacions",
            "ReservacionHabitaciones.Estadia"
        };

        public ReservacionLN(
            IUnidadTrabajoEF unidadTrabajo,
            ILogger<ReservacionLN> logger,
            IMapper mapper)
            : base(unidadTrabajo, mapper, logger)
        {
        }

        #region Consultas

        public async Task<Respuesta<IEnumerable<TReservacion>>> ListarAsync(clsFiltroReservacion filtro)
        {
            try
            {
                var codigo = string.IsNullOrWhiteSpace(filtro.Codigo) ? null : filtro.Codigo.Trim();

                var respuesta = await _unidadDeTrabajo.TReservacion.BuscarAsync(
                    x => (filtro.ClienteId == null || x.ClienteId == filtro.ClienteId)
                      && (filtro.Estado == null || x.Estado == filtro.Estado)
                      && (codigo == null || x.Codigo.Contains(codigo))
                      && (filtro.Hasta == null || x.FechaInicio < filtro.Hasta)
                      && (filtro.Desde == null || x.FechaFin > filtro.Desde),
                    new List<string> { "Cliente.Persona", "ReservacionHabitaciones.Habitacion.Edificio" });

                if (!respuesta.Success)
                    return Falla<IEnumerable<TReservacion>, IEnumerable<Reservacione>>(respuesta);

                var datos = respuesta.Data!
                    .OrderByDescending(x => x.FechaInicio)
                    .ThenByDescending(x => x.Id)
                    .Select(x => Mapear(x, pagado: null, saldo: null, conNoches: false))
                    .ToList();

                return Respuesta<IEnumerable<TReservacion>>.Ok(datos);
            }
            catch (Exception ex)
            {
                return Excepcion<IEnumerable<TReservacion>>(ex, "Error al listar reservaciones.");
            }
        }

        public async Task<Respuesta<TReservacion>> ObtenerAsync(int reservacionId)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.TReservacion.ObtenerEntidadAsync(
                    x => x.Id == reservacionId, IncludesDetalle);

                if (!respuesta.Success)
                    return Falla<TReservacion, Reservacione>(respuesta);

                if (respuesta.Data == null)
                    return Falla<TReservacion>("Reservación no encontrada.", CodigosRespuesta.NoEncontrado);

                decimal? pagado = null, saldo = null;

                if (respuesta.Data.Cuenta != null)
                {
                    var rPagado = await CalcularPagadoAsync(respuesta.Data.Cuenta.Id);
                    if (!rPagado.Success)
                        return Falla<TReservacion, decimal>(rPagado);

                    var rSaldo = await CalcularSaldoAsync(respuesta.Data.Cuenta.Id);
                    if (!rSaldo.Success)
                        return Falla<TReservacion, decimal>(rSaldo);

                    pagado = rPagado.Data;
                    saldo = rSaldo.Data;
                }

                return Respuesta<TReservacion>.Ok(Mapear(respuesta.Data, pagado, saldo, conNoches: true));
            }
            catch (Exception ex)
            {
                return Excepcion<TReservacion>(ex, $"Error al obtener la reservación {reservacionId}.");
            }
        }

        public async Task<Respuesta<IEnumerable<THistorialReservacion>>> ListarHistorialAsync(int reservacionId)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.THistorialReservacion.BuscarAsync(x => x.ReservacionId == reservacionId);
                if (!respuesta.Success)
                    return Falla<IEnumerable<THistorialReservacion>, IEnumerable<HistorialReservacione>>(respuesta);

                var datos = respuesta.Data!
                    .OrderBy(x => x.FechaUtc).ThenBy(x => x.Id)
                    .Select(x => new THistorialReservacion
                    {
                        Id = x.Id,
                        UsuarioId = x.UsuarioId,
                        FechaUtc = x.FechaUtc,
                        EstadoAnterior = x.EstadoAnterior,
                        EstadoNuevo = x.EstadoNuevo,
                        Motivo = x.Motivo
                    });

                return Respuesta<IEnumerable<THistorialReservacion>>.Ok(datos);
            }
            catch (Exception ex)
            {
                return Excepcion<IEnumerable<THistorialReservacion>>(ex, $"Error al listar el historial de la reservación {reservacionId}.");
            }
        }

        #endregion

        #region Crear

        public async Task<Respuesta<TReservacion>> CrearAsync(clsSolicitudReserva solicitud, int usuarioId)
        {
            try
            {
                var observaciones = string.IsNullOrWhiteSpace(solicitud.Observaciones) ? null : solicitud.Observaciones.Trim();
                if (observaciones != null && observaciones.Length > 1000)
                    return Falla<TReservacion>("Las observaciones admiten máximo 1000 caracteres.");

                // Mismo cálculo que api/Reservacion/Cotizar: el precio no lo manda el cliente
                var cotizacion = await CotizarInternoAsync(solicitud);
                if (!cotizacion.Success)
                    return Falla<TReservacion, TCotizacion>(cotizacion);

                var cliente = await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(
                    x => x.Id == solicitud.ClienteId, new List<string> { "Persona" });
                if (!cliente.Success || cliente.Data == null)
                    return Falla<TReservacion>("Cliente no encontrado.", CodigosRespuesta.NoEncontrado);

                var config = await CargarConfiguracionAsync();
                if (!config.Success)
                    return Falla<TReservacion, Dictionary<string, string>>(config);

                var prefijo = ConfigTexto(config.Data!, "PrefijoReserva", "HOT");
                var horasPlazo = ConfigDecimal(config.Data!, "HorasExpiracionPendiente", 24m);

                var ahoraUtc = DateTime.UtcNow;
                var hoy = HoyHotel();
                var consecutivo = await _unidadDeTrabajo.SiguienteCodigoReservaAsync();

                var c = cotizacion.Data!;
                var reservacion = new Reservacione
                {
                    Codigo = $"{prefijo}{hoy:yyyy}-{hoy:MM}-{consecutivo:00000}",
                    ClienteId = c.ClienteId,
                    FechaInicio = c.FechaInicio,
                    FechaFin = c.FechaFin,
                    Estado = (byte)EstadoReservacion.PendientePago,
                    CantidadAdultos = (byte)c.Habitaciones.Sum(x => x.Adultos),
                    CantidadNinos = (byte)c.Habitaciones.Sum(x => x.Ninos),
                    Subtotal = c.Subtotal,
                    Descuento = c.Descuento,
                    Impuestos = c.Impuestos,
                    Total = c.Total,
                    AnticipoRequerido = c.AnticipoRequerido,
                    FechaExpiracionPendienteUtc = ahoraUtc.AddHours((double)horasPlazo),
                    Observaciones = observaciones,
                    CreadoPorUsuarioId = usuarioId,
                    FechaCreacionUtc = ahoraUtc
                };

                foreach (var detalle in ConstruirLineas(c, solicitud, cliente.Data.Persona))
                    reservacion.ReservacionHabitaciones.Add(detalle);

                reservacion.HistorialReservaciones.Add(new HistorialReservacione
                {
                    UsuarioId = usuarioId,
                    FechaUtc = ahoraUtc,
                    EstadoAnterior = null,
                    EstadoNuevo = (byte)EstadoReservacion.PendientePago,
                    Motivo = "Reserva creada"
                });

                // Cuenta de hospedaje donde se registran el anticipo, los cargos y los pagos
                reservacion.Cuenta = new Cuenta
                {
                    ClienteId = c.ClienteId,
                    Tipo = (byte)TipoCuenta.Hospedaje,
                    Estado = (byte)EstadoCuenta.Abierta,
                    FechaAperturaUtc = ahoraUtc
                };

                // Todo el grafo se guarda en un solo SaveChanges: o entra completo o no entra nada
                var creada = await _unidadDeTrabajo.TReservacion.InsertarAsync(reservacion);
                if (!creada.Success)
                {
                    if (creada.Codigo == CodigosRespuesta.Conflicto)
                        return Falla<TReservacion>(
                            "Alguna de las habitaciones se acaba de reservar para esas fechas. Vuelva a consultar la disponibilidad.",
                            CodigosRespuesta.Conflicto);

                    return Falla<TReservacion, Reservacione>(creada);
                }

                return await ObtenerAsync(reservacion.Id);
            }
            catch (Exception ex)
            {
                return Excepcion<TReservacion>(ex, "Error al crear la reservación.");
            }
        }


        // Convierte la cotización en filas de ReservacionHabitaciones con sus noches y huéspedes
        private static List<ReservacionHabitacione> ConstruirLineas(
            TCotizacion cotizacion, clsSolicitudReserva solicitud, Persona personaCliente)
        {
            var lineas = new List<ReservacionHabitacione>();

            for (var i = 0; i < cotizacion.Habitaciones.Count; i++)
            {
                var linea = cotizacion.Habitaciones[i];
                var solicitada = solicitud.Habitaciones.First(x => x.HabitacionId == linea.HabitacionId);

                var detalle = new ReservacionHabitacione
                {
                    HabitacionId = linea.HabitacionId,
                    FechaInicio = linea.FechaInicio,
                    FechaFin = linea.FechaFin,
                    Adultos = linea.Adultos,
                    Ninos = linea.Ninos,
                    Subtotal = linea.Subtotal,
                    Descuento = linea.Descuento,
                    Impuestos = linea.Impuestos,
                    Total = linea.Total
                };

                // Una fila por noche: el índice único (HabitacionId, Fecha) impide dobles reservas
                foreach (var noche in linea.Noches)
                {
                    detalle.ReservacionNoches.Add(new ReservacionNoch
                    {
                        HabitacionId = linea.HabitacionId,
                        Fecha = noche.Fecha,
                        TarifaBase = noche.TarifaBase,
                        AjusteTarifa = noche.AjusteTarifa,
                        TarifaFinal = noche.TarifaFinal,
                        Activa = true
                    });
                }

                foreach (var huesped in solicitada.Huespedes)
                {
                    var sinDocumento = string.IsNullOrWhiteSpace(huesped.NumeroDocumento);
                    detalle.HuespedesReservacions.Add(new HuespedesReservacion
                    {
                        TipoDocumentoId = sinDocumento ? null : huesped.TipoDocumentoId,
                        NumeroDocumento = sinDocumento ? null : huesped.NumeroDocumento!.Trim(),
                        Nombres = huesped.Nombres.Trim(),
                        Apellidos = huesped.Apellidos.Trim(),
                        FechaNacimiento = huesped.FechaNacimiento,
                        EsTitular = huesped.EsTitular
                    });
                }

                // Sin huéspedes indicados: el cliente es el titular de la primera habitación
                if (i == 0 && solicitada.Huespedes.Count == 0)
                {
                    detalle.HuespedesReservacions.Add(new HuespedesReservacion
                    {
                        TipoDocumentoId = personaCliente.NumeroDocumento == null ? null : personaCliente.TipoDocumentoId,
                        NumeroDocumento = personaCliente.NumeroDocumento,
                        Nombres = personaCliente.Nombres,
                        Apellidos = personaCliente.Apellidos,
                        FechaNacimiento = personaCliente.FechaNacimiento,
                        EsTitular = true
                    });
                }

                lineas.Add(detalle);
            }

            return lineas;
        }

        #endregion


        #region Modificar

        /* Cambia fechas, habitaciones o personas de una reserva que aún no tiene check-in.
           Primero libera sus propias noches (para que no choquen consigo mismas), recotiza con las
           tarifas vigentes y reemplaza las líneas. Todo en una transacción: si algo falla, queda como estaba.
           Si el nuevo anticipo supera lo pagado, una reserva confirmada vuelve a PendientePago. */
        public async Task<Respuesta<TReservacion>> ModificarAsync(int reservacionId, clsSolicitudReserva solicitud, int usuarioId)
        {
            try
            {
                var observaciones = string.IsNullOrWhiteSpace(solicitud.Observaciones) ? null : solicitud.Observaciones.Trim();
                if (observaciones != null && observaciones.Length > 1000)
                    return Falla<TReservacion>("Las observaciones admiten máximo 1000 caracteres.");

                var respuesta = await _unidadDeTrabajo.TReservacion.ObtenerEntidadAsync(
                    x => x.Id == reservacionId,
                    new List<string>
                    {
                        "Cliente.Persona",
                        "Cuenta",
                        "ReservacionHabitaciones.ReservacionNoches",
                        "ReservacionHabitaciones.HuespedesReservacions",
                        "ReservacionHabitaciones.Estadia"
                    });

                if (!respuesta.Success)
                    return Falla<TReservacion, Reservacione>(respuesta);
                if (respuesta.Data == null)
                    return Falla<TReservacion>("Reservación no encontrada.", CodigosRespuesta.NoEncontrado);

                var reservacion = respuesta.Data;
                var estadoActual = (EstadoReservacion)reservacion.Estado;

                if (estadoActual is not (EstadoReservacion.PendientePago or EstadoReservacion.Confirmada))
                    return Falla<TReservacion>(
                        $"Solo se modifican reservas pendientes o confirmadas (estado actual: {estadoActual}).",
                        CodigosRespuesta.Conflicto);

                if (reservacion.ReservacionHabitaciones.Any(x => x.Estadia != null))
                    return Falla<TReservacion>("La reserva ya tiene check-in y no se puede modificar.", CodigosRespuesta.Conflicto);

                if (reservacion.Cuenta == null)
                    return Falla<TReservacion>("La reservación no tiene cuenta de hospedaje.", CodigosRespuesta.Error);

                // El cliente de una reserva no cambia
                solicitud.ClienteId = reservacion.ClienteId;

                var config = await CargarConfiguracionAsync();
                if (!config.Success)
                    return Falla<TReservacion, Dictionary<string, string>>(config);

                var pagado = await CalcularPagadoAsync(reservacion.Cuenta.Id);
                if (!pagado.Success)
                    return Falla<TReservacion, decimal>(pagado);

                var antes = $"{reservacion.FechaInicio:yyyy-MM-dd} a {reservacion.FechaFin:yyyy-MM-dd}, "
                          + $"{reservacion.ReservacionHabitaciones.Count} hab., total {reservacion.Total:0.00}";

                _unidadDeTrabajo.EmpezarTransaccion();

                // 1. Liberar las noches propias dentro de la transacción
                foreach (var noche in reservacion.ReservacionHabitaciones.SelectMany(x => x.ReservacionNoches))
                    noche.Activa = false;
                _unidadDeTrabajo.Completar();

                // 2. Recotizar con los datos nuevos (ya no choca consigo misma)
                var cotizacion = await CotizarInternoAsync(solicitud);
                if (!cotizacion.Success)
                {
                    _unidadDeTrabajo.Rollback();
                    return Falla<TReservacion, TCotizacion>(cotizacion);
                }

                // 3. Borrar las líneas viejas (primero noches y huéspedes, que dependen de ellas)
                foreach (var linea in reservacion.ReservacionHabitaciones.ToList())
                {
                    foreach (var noche in linea.ReservacionNoches.ToList())
                    {
                        var borrada = await _unidadDeTrabajo.TReservacionNoche.EliminarAsync(noche);
                        if (!borrada.Success) { _unidadDeTrabajo.Rollback(); return Falla<TReservacion, bool>(borrada); }
                    }

                    foreach (var huesped in linea.HuespedesReservacions.ToList())
                    {
                        var borrado = await _unidadDeTrabajo.THuespedReservacion.EliminarAsync(huesped);
                        if (!borrado.Success) { _unidadDeTrabajo.Rollback(); return Falla<TReservacion, bool>(borrado); }
                    }

                    var lineaBorrada = await _unidadDeTrabajo.TReservacionHabitacion.EliminarAsync(linea);
                    if (!lineaBorrada.Success) { _unidadDeTrabajo.Rollback(); return Falla<TReservacion, bool>(lineaBorrada); }
                }

                // 4. Insertar las líneas nuevas. El índice único de noches rechaza cruces con otras reservas.
                var c = cotizacion.Data!;
                foreach (var detalle in ConstruirLineas(c, solicitud, reservacion.Cliente.Persona))
                {
                    detalle.ReservacionId = reservacion.Id;

                    var insertada = await _unidadDeTrabajo.TReservacionHabitacion.InsertarAsync(detalle);
                    if (!insertada.Success)
                    {
                        _unidadDeTrabajo.Rollback();

                        if (insertada.Codigo == CodigosRespuesta.Conflicto)
                            return Falla<TReservacion>(
                                "Alguna de las habitaciones se acaba de reservar para esas fechas. Vuelva a consultar la disponibilidad.",
                                CodigosRespuesta.Conflicto);

                        return Falla<TReservacion, ReservacionHabitacione>(insertada);
                    }
                }

                // 5. Cabecera con los totales nuevos
                reservacion.FechaInicio = c.FechaInicio;
                reservacion.FechaFin = c.FechaFin;
                reservacion.CantidadAdultos = (byte)c.Habitaciones.Sum(x => x.Adultos);
                reservacion.CantidadNinos = (byte)c.Habitaciones.Sum(x => x.Ninos);
                reservacion.Subtotal = c.Subtotal;
                reservacion.Descuento = c.Descuento;
                reservacion.Impuestos = c.Impuestos;
                reservacion.Total = c.Total;
                reservacion.AnticipoRequerido = c.AnticipoRequerido;
                reservacion.Observaciones = observaciones;

                var estadoNuevo = estadoActual;
                string motivo = "Reserva modificada";

                if (estadoActual == EstadoReservacion.Confirmada && pagado.Data < c.AnticipoRequerido)
                {
                    // El anticipo pagado ya no alcanza: vuelve a pendiente con un plazo nuevo
                    estadoNuevo = EstadoReservacion.PendientePago;
                    var horasPlazo = ConfigDecimal(config.Data!, "HorasExpiracionPendiente", 24m);
                    reservacion.FechaExpiracionPendienteUtc = DateTime.UtcNow.AddHours((double)horasPlazo);
                    motivo = $"Reserva modificada: falta anticipo ({pagado.Data:0.00} de {c.AnticipoRequerido:0.00})";
                }

                var despues = $"{c.FechaInicio:yyyy-MM-dd} a {c.FechaFin:yyyy-MM-dd}, "
                            + $"{c.Habitaciones.Count} hab., total {c.Total:0.00}";

                var cambio = await RegistrarCambioReservaAsync(
                    reservacion, estadoNuevo, usuarioId, motivo, $"Antes: {antes}. Después: {despues}.");
                if (!cambio.Success)
                {
                    _unidadDeTrabajo.Rollback();
                    return Falla<TReservacion, bool>(cambio);
                }

                _unidadDeTrabajo.CompletarTran();

                return await ObtenerAsync(reservacion.Id);
            }
            catch (Exception ex)
            {
                _unidadDeTrabajo.Rollback();
                return Excepcion<TReservacion>(ex, $"Error al modificar la reservación {reservacionId}.");
            }
        }

        #endregion

        #region Confirmar, cancelar y vencer

        public async Task<Respuesta<TReservacion>> ConfirmarAsync(int reservacionId, int usuarioId)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.TReservacion.ObtenerEntidadAsync(
                    x => x.Id == reservacionId,
                    new List<string> { "Cliente.Persona", "Cuenta", "ReservacionHabitaciones.ReservacionNoches" });

                if (!respuesta.Success)
                    return Falla<TReservacion, Reservacione>(respuesta);
                if (respuesta.Data == null)
                    return Falla<TReservacion>("Reservación no encontrada.", CodigosRespuesta.NoEncontrado);

                var reservacion = respuesta.Data;

                if (reservacion.Estado != (byte)EstadoReservacion.PendientePago)
                    return Falla<TReservacion>(
                        $"Solo se confirman reservas pendientes de pago (estado actual: {(EstadoReservacion)reservacion.Estado}).",
                        CodigosRespuesta.Conflicto);

                if (reservacion.FechaExpiracionPendienteUtc < DateTime.UtcNow)
                {
                    var vencida = await CancelarInternoAsync(
                        reservacion, "Vencida: el anticipo no se pagó a tiempo", reservacion.CreadoPorUsuarioId);
                    if (!vencida.Success)
                        return Falla<TReservacion, bool>(vencida);

                    return Falla<TReservacion>("El plazo para pagar el anticipo venció y la reserva se canceló.", CodigosRespuesta.Conflicto);
                }

                if (reservacion.Cuenta == null)
                    return Falla<TReservacion>("La reservación no tiene cuenta de hospedaje.", CodigosRespuesta.Error);

                // Integración con el módulo económico: el anticipo se paga en Pagos sobre la cuenta
                var pagado = await CalcularPagadoAsync(reservacion.Cuenta.Id);
                if (!pagado.Success)
                    return Falla<TReservacion, decimal>(pagado);

                if (pagado.Data < reservacion.AnticipoRequerido)
                    return Falla<TReservacion>(
                        $"Falta el anticipo: se requieren {reservacion.AnticipoRequerido:0.00} y hay pagado {pagado.Data:0.00} "
                        + $"(cuenta {reservacion.Cuenta.Id}).",
                        CodigosRespuesta.Conflicto);

                _unidadDeTrabajo.EmpezarTransaccion();

                reservacion.FechaExpiracionPendienteUtc = null;

                var cambio = await RegistrarCambioReservaAsync(
                    reservacion, EstadoReservacion.Confirmada, usuarioId, $"Anticipo cubierto: {pagado.Data:0.00}");
                if (!cambio.Success)
                {
                    _unidadDeTrabajo.Rollback();
                    return Falla<TReservacion, bool>(cambio);
                }

                var aviso = await EncolarCorreoAsync(reservacion, TipoNotificacionCorreo.ReservaConfirmada);
                if (!aviso.Success)
                {
                    _unidadDeTrabajo.Rollback();
                    return Falla<TReservacion, bool>(aviso);
                }

                _unidadDeTrabajo.CompletarTran();

                return await ObtenerAsync(reservacion.Id);
            }
            catch (Exception ex)
            {
                _unidadDeTrabajo.Rollback();
                return Excepcion<TReservacion>(ex, $"Error al confirmar la reservación {reservacionId}.");
            }
        }

        public async Task<Respuesta<TReservacion>> CancelarAsync(int reservacionId, string motivo, int usuarioId)
        {
            try
            {
                motivo = motivo?.Trim() ?? string.Empty;
                if (motivo.Length == 0 || motivo.Length > 500)
                    return Falla<TReservacion>("El motivo de cancelación es obligatorio (máximo 500 caracteres).");

                var respuesta = await _unidadDeTrabajo.TReservacion.ObtenerEntidadAsync(
                    x => x.Id == reservacionId,
                    new List<string> { "Cliente.Persona", "ReservacionHabitaciones.ReservacionNoches" });

                if (!respuesta.Success)
                    return Falla<TReservacion, Reservacione>(respuesta);
                if (respuesta.Data == null)
                    return Falla<TReservacion>("Reservación no encontrada.", CodigosRespuesta.NoEncontrado);

                var estado = (EstadoReservacion)respuesta.Data.Estado;
                if (estado is not (EstadoReservacion.PendientePago or EstadoReservacion.Confirmada))
                    return Falla<TReservacion>(
                        $"No se puede cancelar una reserva en estado {estado}. Si ya hizo check-in, corresponde check-out.",
                        CodigosRespuesta.Conflicto);

                var cancelada = await CancelarInternoAsync(respuesta.Data, motivo, usuarioId);
                if (!cancelada.Success)
                    return Falla<TReservacion, bool>(cancelada);

                return await ObtenerAsync(reservacionId);
            }
            catch (Exception ex)
            {
                return Excepcion<TReservacion>(ex, $"Error al cancelar la reservación {reservacionId}.");
            }
        }

        public async Task<Respuesta<int>> VencerPendientesAsync()
        {
            try
            {
                var ahora = DateTime.UtcNow;
                var pendiente = (byte)EstadoReservacion.PendientePago;

                var vencidas = await _unidadDeTrabajo.TReservacion.BuscarAsync(
                    x => x.Estado == pendiente && x.FechaExpiracionPendienteUtc < ahora);

                if (!vencidas.Success)
                    return Falla<int, IEnumerable<Reservacione>>(vencidas);

                var canceladas = 0;
                foreach (var id in vencidas.Data!.Select(x => x.Id).ToList())
                {
                    // Se vuelve a cargar cada una: si una falla, el Rollback limpia el contexto
                    var reservacion = await _unidadDeTrabajo.TReservacion.ObtenerEntidadAsync(
                        x => x.Id == id && x.Estado == pendiente,
                        new List<string> { "Cliente.Persona", "ReservacionHabitaciones.ReservacionNoches" });

                    if (!reservacion.Success || reservacion.Data == null)
                        continue;

                    var resultado = await CancelarInternoAsync(
                        reservacion.Data, "Vencida: el anticipo no se pagó a tiempo", reservacion.Data.CreadoPorUsuarioId);

                    if (resultado.Success)
                        canceladas++;
                    else
                        _logger.LogWarning("No se pudo vencer la reserva {Codigo}: {Error}", reservacion.Data.Codigo, resultado.Error);
                }

                return Respuesta<int>.Ok(canceladas);
            }
            catch (Exception ex)
            {
                return Excepcion<int>(ex, "Error al vencer reservas pendientes.");
            }
        }

        // Cancela y libera las noches. Requiere la reserva con ReservacionHabitaciones.ReservacionNoches.
        private async Task<Respuesta<bool>> CancelarInternoAsync(Reservacione reservacion, string motivo, int usuarioId)
        {
            try
            {
                _unidadDeTrabajo.EmpezarTransaccion();

                reservacion.MotivoCancelacion = motivo.Length > 500 ? motivo[..500] : motivo;

                foreach (var noche in reservacion.ReservacionHabitaciones.SelectMany(x => x.ReservacionNoches))
                    noche.Activa = false;

                var cambio = await RegistrarCambioReservaAsync(reservacion, EstadoReservacion.Cancelada, usuarioId, motivo);
                if (!cambio.Success)
                {
                    _unidadDeTrabajo.Rollback();
                    return cambio;
                }

                var aviso = await EncolarCorreoAsync(reservacion, TipoNotificacionCorreo.Cancelacion);
                if (!aviso.Success)
                {
                    _unidadDeTrabajo.Rollback();
                    return aviso;
                }

                _unidadDeTrabajo.CompletarTran();
                return Respuesta<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                _unidadDeTrabajo.Rollback();
                return Excepcion<bool>(ex, $"Error al cancelar la reservación {reservacion.Id}.");
            }
        }

        // Deja el aviso en NotificacionesCorreo; el servicio de correo (compañero 3) lo envía
        private async Task<Respuesta<bool>> EncolarCorreoAsync(Reservacione reservacion, TipoNotificacionCorreo tipo)
        {
            var correo = reservacion.Cliente?.Persona?.EmailContacto;
            if (string.IsNullOrWhiteSpace(correo))
                return Respuesta<bool>.Ok(false); // sin correo registrado no hay a quién avisar

            var insertado = await _unidadDeTrabajo.TNotificacionCorreo.InsertarAsync(new NotificacionesCorreo
            {
                ClienteId = reservacion.ClienteId,
                Tipo = (byte)tipo,
                ReservacionId = reservacion.Id,
                Destinatario = correo,
                Estado = (byte)EstadoCorreo.Pendiente,
                Intentos = 0,
                FechaCreacionUtc = DateTime.UtcNow
            });

            return insertado.Success ? Respuesta<bool>.Ok(true) : Falla<bool, NotificacionesCorreo>(insertado);
        }

        #endregion

        #region Mapeo

        private static TReservacion Mapear(Reservacione r, decimal? pagado, decimal? saldo, bool conNoches)
        {
            return new TReservacion
            {
                Id = r.Id,
                Codigo = r.Codigo,
                ClienteId = r.ClienteId,
                ClienteNombre = NombreCliente(r.Cliente),
                FechaInicio = r.FechaInicio,
                FechaFin = r.FechaFin,
                Noches = r.FechaFin.DayNumber - r.FechaInicio.DayNumber,
                Estado = r.Estado,
                EstadoNombre = ((EstadoReservacion)r.Estado).ToString(),
                CantidadAdultos = r.CantidadAdultos,
                CantidadNinos = r.CantidadNinos,
                Subtotal = r.Subtotal,
                Descuento = r.Descuento,
                Impuestos = r.Impuestos,
                Total = r.Total,
                AnticipoRequerido = r.AnticipoRequerido,
                FechaExpiracionPendienteUtc = r.FechaExpiracionPendienteUtc,
                Observaciones = r.Observaciones,
                MotivoCancelacion = r.MotivoCancelacion,
                CreadoPorUsuarioId = r.CreadoPorUsuarioId,
                FechaCreacionUtc = r.FechaCreacionUtc,
                CuentaId = r.Cuenta?.Id,
                MontoPagado = pagado,
                Saldo = saldo,
                Habitaciones = r.ReservacionHabitaciones
                    .OrderBy(x => x.Id)
                    .Select(h => new TReservacionHabitacion
                    {
                        Id = h.Id,
                        HabitacionId = h.HabitacionId,
                        Habitacion = h.Habitacion != null ? NombreHabitacion(h.Habitacion) : null,
                        TipoHabitacionNombre = h.Habitacion?.TipoHabitacion?.Nombre,
                        FechaInicio = h.FechaInicio,
                        FechaFin = h.FechaFin,
                        Adultos = h.Adultos,
                        Ninos = h.Ninos,
                        Subtotal = h.Subtotal,
                        Descuento = h.Descuento,
                        Impuestos = h.Impuestos,
                        Total = h.Total,
                        EstadiaId = h.Estadia?.Id,
                        EstadoEstadia = h.Estadia?.Estado,
                        Noches = !conNoches ? new() : h.ReservacionNoches
                            .OrderBy(n => n.Fecha)
                            .Select(n => new TReservacionNoche
                            {
                                Fecha = n.Fecha,
                                TarifaBase = n.TarifaBase,
                                AjusteTarifa = n.AjusteTarifa,
                                TarifaFinal = n.TarifaFinal
                            }).ToList(),
                        Huespedes = !conNoches ? new() : h.HuespedesReservacions
                            .Select(x => new THuespedReservacion
                            {
                                Id = x.Id,
                                TipoDocumentoId = x.TipoDocumentoId,
                                NumeroDocumento = x.NumeroDocumento,
                                Nombres = x.Nombres,
                                Apellidos = x.Apellidos,
                                FechaNacimiento = x.FechaNacimiento,
                                EsTitular = x.EsTitular
                            }).ToList()
                    }).ToList()
            };
        }

        #endregion
    }
}
