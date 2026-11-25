using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Utilitarios;

namespace SistemaHotelero.Dominio.InterfazLN;

public interface IReglaTarifaLN
{
    Task<Respuesta<IEnumerable<TReglaTarifa>>> ListarAsync(bool soloActivos);
    Task<Respuesta<TReglaTarifa>> ObtenerAsync(TReglaTarifa datos);
    Task<Respuesta<TReglaTarifa>> CrearAsync(TReglaTarifa datos);
    Task<Respuesta<TReglaTarifa>> ModificarAsync(TReglaTarifa datos);
    Task<Respuesta<TReglaTarifa>> CambiarActivoAsync(TReglaTarifa datos);
}
