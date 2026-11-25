using AutoMapper;
using Microsoft.Extensions.Logging;
using SistemaHotelero.Dominio.InterfazAD;
using SistemaHotelero.Utilitarios;

namespace SistemaHotelero.LogicaNegocio.Implementacion
{
    public abstract class BaseLN<T>
    {
        protected readonly IUnidadTrabajoEF _unidadDeTrabajo;
        protected readonly IMapper _mapper;
        protected readonly ILogger<T> _logger;

        protected BaseLN(
            IUnidadTrabajoEF unidadTrabajo, IMapper mapper, ILogger<T> logger)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _mapper = mapper;
            _logger = logger;
        }

        // Atajos para no repetir las mismas tres líneas en cada validación
        protected static Respuesta<TData> Falla<TData>(string error, string codigo = CodigosRespuesta.Validacion)
            => Respuesta<TData>.Fail(error, codigo);

        protected static Respuesta<TData> Falla<TData, TOrigen>(Respuesta<TOrigen> origen)
            => Respuesta<TData>.Fail(origen.Error, origen.Codigo ?? CodigosRespuesta.Error);

        #region Fechas del hotel (Costa Rica)

        protected static TimeZoneInfo ZonaHotel()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("America/Costa_Rica");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
            }
        }

        protected static DateTime AhoraHotel()
            => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ZonaHotel());

        // Fecha de hoy en Costa Rica, sin importar la zona horaria del servidor
        protected static DateOnly HoyHotel()
            => DateOnly.FromDateTime(AhoraHotel());

        // Fecha y hora local del hotel convertidas a UTC (para guardar en columnas *Utc)
        protected static DateTime LocalHotelAUtc(DateOnly fecha, TimeOnly hora)
            => TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(fecha.ToDateTime(hora), DateTimeKind.Unspecified), ZonaHotel());

        #endregion

        protected Respuesta<TData> Excepcion<TData>(Exception ex, string mensaje)
        {
            _logger.LogError(ex, "{Mensaje}", mensaje);
            return Respuesta<TData>.Fail(ex.GetBaseException().Message, CodigosRespuesta.Error);
        }
    }
}
