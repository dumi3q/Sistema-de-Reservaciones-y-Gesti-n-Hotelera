using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Utilitarios;

namespace SistemaHotelero.Dominio.InterfazLN;

public interface IClienteLN
{
    // texto busca en nombres, apellidos, documento y correo
    Task<Respuesta<IEnumerable<TCliente>>> ListarAsync(string? texto, bool? activo);
    Task<Respuesta<TCliente>> ObtenerAsync(TCliente datos);
    Task<Respuesta<TCliente>> CrearAsync(TCliente datos);
    Task<Respuesta<TCliente>> ModificarAsync(TCliente datos);
    Task<Respuesta<TCliente>> CambiarActivoAsync(TCliente datos);
}
