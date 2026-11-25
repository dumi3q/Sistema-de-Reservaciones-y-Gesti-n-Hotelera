using SistemaHotelero.Dominio.Clases;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Utilitarios;

namespace SistemaHotelero.Dominio.InterfazLN;

public interface IHabitacionLN
{
    Task<Respuesta<IEnumerable<THabitacion>>> ListarAsync(clsFiltroHabitacion filtro);
    Task<Respuesta<THabitacion>> ObtenerAsync(THabitacion datos);
    Task<Respuesta<THabitacion>> CrearAsync(THabitacion datos);
    Task<Respuesta<THabitacion>> ModificarAsync(THabitacion datos);

    // Cambios manuales del estado operativo. Reservada y Ocupada los ponen reservas y check-in.
    Task<Respuesta<THabitacion>> CambiarEstadoAsync(int habitacionId, clsCambioEstadoHabitacion datos, int usuarioId);
    Task<Respuesta<IEnumerable<THistorialEstadoHabitacion>>> ListarHistorialEstadosAsync(int habitacionId);

    // Reemplaza la lista completa de amenidades de la habitación
    Task<Respuesta<THabitacion>> AsignarAmenidadesAsync(int habitacionId, List<clsAmenidadAsignada> amenidades);

    Task<Respuesta<TFotoHabitacion>> AgregarFotoAsync(TFotoHabitacion datos);
    Task<Respuesta<bool>> EliminarFotoAsync(int fotoId);
    Task<Respuesta<TFotoHabitacion>> MarcarFotoPrincipalAsync(int fotoId);
}

public interface IBloqueoHabitacionLN
{
    Task<Respuesta<IEnumerable<TBloqueoHabitacion>>> ListarAsync(int? habitacionId, bool soloActivos);
    Task<Respuesta<TBloqueoHabitacion>> CrearAsync(TBloqueoHabitacion datos, int usuarioId);
    Task<Respuesta<TBloqueoHabitacion>> DesactivarAsync(int bloqueoId);
}
