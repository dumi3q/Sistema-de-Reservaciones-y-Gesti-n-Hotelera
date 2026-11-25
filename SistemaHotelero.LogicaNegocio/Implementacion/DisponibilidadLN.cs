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
    public class DisponibilidadLN : BaseHospedajeLN<DisponibilidadLN>, IDisponibilidadLN
    {
        public DisponibilidadLN(
            IUnidadTrabajoEF unidadTrabajo,
            ILogger<DisponibilidadLN> logger,
            IMapper mapper)
            : base(unidadTrabajo, mapper, logger)
        {
        }

        public async Task<Respuesta<IEnumerable<TDisponibilidadHabitacion>>> BuscarAsync(clsFiltroDisponibilidad filtro)
        {
            try
            {
                var errorFechas = ValidarFechas(filtro.FechaInicio, filtro.FechaFin);
                if (errorFechas != null)
                    return Falla<IEnumerable<TDisponibilidadHabitacion>>(errorFechas);

                if (filtro.Adultos < 1)
                    return Falla<IEnumerable<TDisponibilidadHabitacion>>("Debe haber al menos un adulto.");

                var adultos = filtro.Adultos;
                var ninos = filtro.Ninos;
                var personas = adultos + ninos;
                var fueraDeServicio = (byte)EstadoHabitacion.FueraDeServicio;

                // Candidatas: capacidad, tipo y edificio activos, y los filtros opcionales
                var candidatas = await _unidadDeTrabajo.THabitacion.BuscarAsync(
                    x => x.Estado != fueraDeServicio
                      && x.Edificio.Activo
                      && x.TipoHabitacion.Activo
                      && x.TipoHabitacion.CapacidadAdultos >= adultos
                      && x.TipoHabitacion.CapacidadNinos >= ninos
                      && x.TipoHabitacion.CapacidadMaxima >= personas
                      && (filtro.TipoHabitacionId == null || x.TipoHabitacionId == filtro.TipoHabitacionId)
                      && (filtro.EdificioId == null || x.EdificioId == filtro.EdificioId),
                    new List<string> { "Edificio", "TipoHabitacion" });

                if (!candidatas.Success)
                    return Falla<IEnumerable<TDisponibilidadHabitacion>, IEnumerable<Habitacione>>(candidatas);

                var noLibres = await HabitacionesNoLibresAsync(filtro.FechaInicio, filtro.FechaFin);
                if (!noLibres.Success)
                    return Falla<IEnumerable<TDisponibilidadHabitacion>, HashSet<int>>(noLibres);

                var reglas = await CargarReglasAsync();
                if (!reglas.Success)
                    return Falla<IEnumerable<TDisponibilidadHabitacion>, List<ReglasTarifa>>(reglas);

                var cantidadNoches = filtro.FechaFin.DayNumber - filtro.FechaInicio.DayNumber;

                // Todas las habitaciones del mismo tipo cuestan igual: se calcula una vez por tipo
                var subtotalPorTipo = new Dictionary<int, decimal>();

                var resultado = new List<TDisponibilidadHabitacion>();

                foreach (var habitacion in candidatas.Data!)
                {
                    if (noLibres.Data!.Contains(habitacion.Id) || !EstadoPermiteReservar(habitacion, filtro.FechaInicio))
                        continue;

                    if (!subtotalPorTipo.TryGetValue(habitacion.TipoHabitacionId, out var subtotal))
                    {
                        subtotal = CalcularNoches(habitacion.TipoHabitacion, personas,
                                filtro.FechaInicio, filtro.FechaFin, reglas.Data!)
                            .Sum(x => x.TarifaFinal);
                        subtotalPorTipo[habitacion.TipoHabitacionId] = subtotal;
                    }

                    resultado.Add(new TDisponibilidadHabitacion
                    {
                        HabitacionId = habitacion.Id,
                        EdificioId = habitacion.EdificioId,
                        Habitacion = NombreHabitacion(habitacion),
                        Piso = habitacion.Piso,
                        TipoHabitacionId = habitacion.TipoHabitacionId,
                        TipoHabitacionNombre = habitacion.TipoHabitacion.Nombre,
                        CapacidadMaxima = habitacion.TipoHabitacion.CapacidadMaxima,
                        Noches = cantidadNoches,
                        SubtotalEstimado = subtotal,
                        PrecioPromedioNoche = Redondear(subtotal / cantidadNoches)
                    });
                }

                return Respuesta<IEnumerable<TDisponibilidadHabitacion>>.Ok(
                    resultado.OrderBy(x => x.SubtotalEstimado).ThenBy(x => x.Habitacion));
            }
            catch (Exception ex)
            {
                return Excepcion<IEnumerable<TDisponibilidadHabitacion>>(ex, "Error al buscar disponibilidad.");
            }
        }

        public async Task<Respuesta<TCotizacion>> CotizarAsync(clsSolicitudReserva solicitud)
        {
            try
            {
                return await CotizarInternoAsync(solicitud);
            }
            catch (Exception ex)
            {
                return Excepcion<TCotizacion>(ex, "Error al cotizar la reserva.");
            }
        }
    }
}
