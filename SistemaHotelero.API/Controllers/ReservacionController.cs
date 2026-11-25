using Microsoft.AspNetCore.Mvc;
using SistemaHotelero.API.Extensiones;
using SistemaHotelero.Dominio.Clases;
using SistemaHotelero.Dominio.InterfazLN;

namespace SistemaHotelero.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservacionController : ControllerBase
{
    private readonly IReservacionLN _reservacionLN;
    private readonly IDisponibilidadLN _disponibilidadLN;

    public ReservacionController(IReservacionLN reservacionLN, IDisponibilidadLN disponibilidadLN)
    {
        _reservacionLN = reservacionLN;
        _disponibilidadLN = disponibilidadLN;
    }

    // GET api/Reservacion/Listar?clienteId=1&estado=2&desde=2026-12-01&hasta=2026-12-31&codigo=HOT
    [HttpGet("Listar")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Listar([FromQuery] clsFiltroReservacion filtro)
        => this.Responder(await _reservacionLN.ListarAsync(filtro));

    // Incluye habitaciones, noches, huéspedes, pagado y saldo de la cuenta
    [HttpGet("Obtener/{id:int}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Obtener(int id)
        => this.Responder(await _reservacionLN.ObtenerAsync(id));

    [HttpGet("Historial/{id:int}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Historial(int id)
        => this.Responder(await _reservacionLN.ListarHistorialAsync(id));

    // Calcula el precio sin guardar nada
    [HttpPost("Cotizar")]
    public async Task<IActionResult> Cotizar([FromBody] clsSolicitudReserva solicitud)
        => this.Responder(await _disponibilidadLN.CotizarAsync(solicitud));

    // Queda PendientePago; el anticipo se paga en la cuenta que devuelve (CuentaId)
    [HttpPost("Crear")]
    public async Task<IActionResult> Crear([FromBody] clsSolicitudReserva solicitud)
    {
        var usuarioId = this.ObtenerUsuarioId();
        if (usuarioId == null)
            return this.SinUsuario();

        return this.Responder(await _reservacionLN.CrearAsync(solicitud, usuarioId.Value));
    }

    // Mismo cuerpo que Crear (el cliente no cambia). Solo antes del check-in.
    [HttpPut("Modificar/{id:int}")]
    public async Task<IActionResult> Modificar(int id, [FromBody] clsSolicitudReserva solicitud)
    {
        var usuarioId = this.ObtenerUsuarioId();
        if (usuarioId == null)
            return this.SinUsuario();

        return this.Responder(await _reservacionLN.ModificarAsync(id, solicitud, usuarioId.Value));
    }

    [HttpPost("Confirmar/{id:int}")]
    public async Task<IActionResult> Confirmar(int id)
    {
        var usuarioId = this.ObtenerUsuarioId();
        if (usuarioId == null)
            return this.SinUsuario();

        return this.Responder(await _reservacionLN.ConfirmarAsync(id, usuarioId.Value));
    }

    [HttpPost("Cancelar/{id:int}")]
    public async Task<IActionResult> Cancelar(int id, [FromBody] clsCancelacion datos)
    {
        var usuarioId = this.ObtenerUsuarioId();
        if (usuarioId == null)
            return this.SinUsuario();

        return this.Responder(await _reservacionLN.CancelarAsync(id, datos.Motivo, usuarioId.Value));
    }
}
