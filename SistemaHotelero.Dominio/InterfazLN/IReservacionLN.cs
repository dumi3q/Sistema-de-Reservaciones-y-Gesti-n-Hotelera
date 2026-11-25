using SistemaHotelero.Dominio.Clases;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Utilitarios;

namespace SistemaHotelero.Dominio.InterfazLN;

public interface IDisponibilidadLN
{
    Task<Respuesta<IEnumerable<TDisponibilidadHabitacion>>> BuscarAsync(clsFiltroDisponibilidad filtro);

    // Calcula precios en el servidor sin guardar nada
    Task<Respuesta<TCotizacion>> CotizarAsync(clsSolicitudReserva solicitud);
}

public interface IReservacionLN
{
    Task<Respuesta<IEnumerable<TReservacion>>> ListarAsync(clsFiltroReservacion filtro);
    Task<Respuesta<TReservacion>> ObtenerAsync(int reservacionId);
    Task<Respuesta<IEnumerable<THistorialReservacion>>> ListarHistorialAsync(int reservacionId);

    // Queda PendientePago hasta que se pague el anticipo
    Task<Respuesta<TReservacion>> CrearAsync(clsSolicitudReserva solicitud, int usuarioId);

    // Cambia fechas, habitaciones o personas antes del check-in (recotiza con tarifas vigentes)
    Task<Respuesta<TReservacion>> ModificarAsync(int reservacionId, clsSolicitudReserva solicitud, int usuarioId);

    // Verifica en Pagos que el anticipo esté cubierto
    Task<Respuesta<TReservacion>> ConfirmarAsync(int reservacionId, int usuarioId);
    Task<Respuesta<TReservacion>> CancelarAsync(int reservacionId, string motivo, int usuarioId);

    // Cancela las PendientePago cuyo plazo venció. Devuelve cuántas canceló.
    Task<Respuesta<int>> VencerPendientesAsync();
}

public interface IEstadiaLN
{
    Task<Respuesta<TCheckIn>> CheckInAsync(int reservacionId, int usuarioId);
    Task<Respuesta<TCheckOut>> CheckOutAsync(int reservacionId, int usuarioId);
    Task<Respuesta<IEnumerable<TEstadia>>> ListarActivasAsync();

    // Para restaurante (compañero 3): validar que la habitación tenga una estadía activa
    Task<Respuesta<TEstadia>> ObtenerActivaPorHabitacionAsync(int habitacionId);
}
