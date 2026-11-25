using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Utilitarios;

namespace SistemaHotelero.Dominio.InterfazLN;

public interface IEdificioLN
{
    Task<Respuesta<IEnumerable<TEdificio>>> ListarAsync();
    Task<Respuesta<TEdificio>> ObtenerAsync(TEdificio datos);
    Task<Respuesta<TEdificio>> ModificarAsync(TEdificio datos);
}

public interface ITipoHabitacionLN
{
    Task<Respuesta<IEnumerable<TTipoHabitacion>>> ListarAsync(bool soloActivos);
    Task<Respuesta<TTipoHabitacion>> ObtenerAsync(TTipoHabitacion datos);
    Task<Respuesta<TTipoHabitacion>> CrearAsync(TTipoHabitacion datos);
    Task<Respuesta<TTipoHabitacion>> ModificarAsync(TTipoHabitacion datos);
    Task<Respuesta<TTipoHabitacion>> CambiarActivoAsync(TTipoHabitacion datos);
}

public interface IAmenidadLN
{
    Task<Respuesta<IEnumerable<TAmenidad>>> ListarAsync(bool soloActivos);
    Task<Respuesta<TAmenidad>> ObtenerAsync(TAmenidad datos);
    Task<Respuesta<TAmenidad>> CrearAsync(TAmenidad datos);
    Task<Respuesta<TAmenidad>> ModificarAsync(TAmenidad datos);
    Task<Respuesta<TAmenidad>> CambiarActivoAsync(TAmenidad datos);
}
