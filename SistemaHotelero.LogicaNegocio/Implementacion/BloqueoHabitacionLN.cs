using AutoMapper;
using Microsoft.Extensions.Logging;
using SistemaHotelero.Dominio.Entidades;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.Enumeraciones;
using SistemaHotelero.Dominio.InterfazAD;
using SistemaHotelero.Dominio.InterfazLN;
using SistemaHotelero.Utilitarios;

namespace SistemaHotelero.LogicaNegocio.Implementacion
{
    // Bloqueos por fechas: la habitación no se ofrece en disponibilidad durante [FechaInicio, FechaFin)
    public class BloqueoHabitacionLN : BaseLN<BloqueoHabitacionLN>, IBloqueoHabitacionLN
    {
        public BloqueoHabitacionLN(
            IUnidadTrabajoEF unidadTrabajo,
            ILogger<BloqueoHabitacionLN> logger,
            IMapper mapper)
            : base(unidadTrabajo, mapper, logger)
        {
        }

        public async Task<Respuesta<IEnumerable<TBloqueoHabitacion>>> ListarAsync(int? habitacionId, bool soloActivos)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.TBloqueoHabitacion.BuscarAsync(
                    x => (habitacionId == null || x.HabitacionId == habitacionId)
                      && (!soloActivos || x.Activo));

                if (!respuesta.Success)
                    return Falla<IEnumerable<TBloqueoHabitacion>, IEnumerable<BloqueosHabitacion>>(respuesta);

                return Respuesta<IEnumerable<TBloqueoHabitacion>>.Ok(
                    _mapper.Map<IEnumerable<TBloqueoHabitacion>>(respuesta.Data).OrderBy(x => x.FechaInicio));
            }
            catch (Exception ex)
            {
                return Excepcion<IEnumerable<TBloqueoHabitacion>>(ex, "Error al listar bloqueos.");
            }
        }

        public async Task<Respuesta<TBloqueoHabitacion>> CrearAsync(TBloqueoHabitacion datos, int usuarioId)
        {
            try
            {
                datos.Motivo = datos.Motivo?.Trim() ?? string.Empty;

                if (datos.Motivo.Length == 0 || datos.Motivo.Length > 300)
                    return Falla<TBloqueoHabitacion>("El motivo es obligatorio (máximo 300 caracteres).");

                if (!Enum.IsDefined(typeof(TipoBloqueo), datos.Tipo))
                    return Falla<TBloqueoHabitacion>("El tipo debe ser 1 (Mantenimiento) o 2 (Bloqueo).");

                if (datos.FechaInicio >= datos.FechaFin)
                    return Falla<TBloqueoHabitacion>("La fecha de inicio debe ser anterior a la fecha de fin.");

                if (datos.FechaInicio < HoyHotel())
                    return Falla<TBloqueoHabitacion>("No se pueden crear bloqueos en fechas pasadas.");

                var existe = await _unidadDeTrabajo.THabitacion.ContarAsync(x => x.Id == datos.HabitacionId);
                if (!existe.Success)
                    return Falla<TBloqueoHabitacion, int?>(existe);
                if (existe.Data == 0)
                    return Falla<TBloqueoHabitacion>("Habitación no encontrada.", CodigosRespuesta.NoEncontrado);

                // Dos intervalos [a, b) y [c, d) se cruzan si a < d y c < b
                var cruceBloqueo = await _unidadDeTrabajo.TBloqueoHabitacion.ContarAsync(
                    x => x.HabitacionId == datos.HabitacionId
                      && x.Activo
                      && x.FechaInicio < datos.FechaFin
                      && datos.FechaInicio < x.FechaFin);
                if (!cruceBloqueo.Success)
                    return Falla<TBloqueoHabitacion, int?>(cruceBloqueo);
                if (cruceBloqueo.Data > 0)
                    return Falla<TBloqueoHabitacion>(
                        "Ya hay un bloqueo activo que se cruza con esas fechas.", CodigosRespuesta.Conflicto);

                // No se bloquea encima de noches ya reservadas: primero hay que mover esas reservas
                var nochesReservadas = await _unidadDeTrabajo.TReservacionNoche.ContarAsync(
                    x => x.HabitacionId == datos.HabitacionId
                      && x.Activa
                      && x.Fecha >= datos.FechaInicio
                      && x.Fecha < datos.FechaFin);
                if (!nochesReservadas.Success)
                    return Falla<TBloqueoHabitacion, int?>(nochesReservadas);
                if (nochesReservadas.Data > 0)
                    return Falla<TBloqueoHabitacion>(
                        $"La habitación tiene {nochesReservadas.Data} noche(s) reservada(s) en ese rango.",
                        CodigosRespuesta.Conflicto);

                var bloqueo = new BloqueosHabitacion
                {
                    HabitacionId = datos.HabitacionId,
                    FechaInicio = datos.FechaInicio,
                    FechaFin = datos.FechaFin,
                    Tipo = datos.Tipo,
                    Motivo = datos.Motivo,
                    UsuarioId = usuarioId,
                    Activo = true
                };

                var creado = await _unidadDeTrabajo.TBloqueoHabitacion.InsertarAsync(bloqueo);
                if (!creado.Success)
                    return Falla<TBloqueoHabitacion, BloqueosHabitacion>(creado);

                return Respuesta<TBloqueoHabitacion>.Ok(_mapper.Map<TBloqueoHabitacion>(bloqueo));
            }
            catch (Exception ex)
            {
                return Excepcion<TBloqueoHabitacion>(ex, "Error al crear el bloqueo.");
            }
        }

        public async Task<Respuesta<TBloqueoHabitacion>> DesactivarAsync(int bloqueoId)
        {
            try
            {
                var actual = await _unidadDeTrabajo.TBloqueoHabitacion.ObtenerEntidadAsync(x => x.Id == bloqueoId);
                if (!actual.Success)
                    return Falla<TBloqueoHabitacion, BloqueosHabitacion>(actual);
                if (actual.Data == null)
                    return Falla<TBloqueoHabitacion>("Bloqueo no encontrado.", CodigosRespuesta.NoEncontrado);

                if (!actual.Data.Activo)
                    return Falla<TBloqueoHabitacion>("El bloqueo ya estaba desactivado.");

                actual.Data.Activo = false;

                var guardado = await _unidadDeTrabajo.TBloqueoHabitacion.ModificarAsync(actual.Data);
                if (!guardado.Success)
                    return Falla<TBloqueoHabitacion, BloqueosHabitacion>(guardado);

                return Respuesta<TBloqueoHabitacion>.Ok(_mapper.Map<TBloqueoHabitacion>(actual.Data));
            }
            catch (Exception ex)
            {
                return Excepcion<TBloqueoHabitacion>(ex, $"Error al desactivar el bloqueo {bloqueoId}.");
            }
        }
    }
}
