using Microsoft.AspNetCore.Mvc;
using SistemaHotelero.API.Extensiones;
using SistemaHotelero.Dominio.InterfazLN;

namespace SistemaHotelero.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EstadiaController : ControllerBase
{
    private readonly IEstadiaLN _estadiaLN;

    public EstadiaController(IEstadiaLN estadiaLN)
    {
        _estadiaLN = estadiaLN;
    }

    // Devuelve los códigos de acceso de cada habitación (solo se muestran esta vez)
    [HttpPost("CheckIn/{reservacionId:int}")]
    public async Task<IActionResult> CheckIn(int reservacionId)
    {
        var usuarioId = this.ObtenerUsuarioId();
        if (usuarioId == null)
            return this.SinUsuario();

        return this.Responder(await _estadiaLN.CheckInAsync(reservacionId, usuarioId.Value));
    }

    // Requiere saldo cero en la cuenta de hospedaje
    [HttpPost("CheckOut/{reservacionId:int}")]
    public async Task<IActionResult> CheckOut(int reservacionId)
    {
        var usuarioId = this.ObtenerUsuarioId();
        if (usuarioId == null)
            return this.SinUsuario();

        return this.Responder(await _estadiaLN.CheckOutAsync(reservacionId, usuarioId.Value));
    }

    [HttpGet("Activas")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Activas()
        => this.Responder(await _estadiaLN.ListarActivasAsync());

    // Para restaurante: ¿la habitación tiene huésped? (404 si no)
    [HttpGet("ActivaPorHabitacion/{habitacionId:int}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> ActivaPorHabitacion(int habitacionId)
        => this.Responder(await _estadiaLN.ObtenerActivaPorHabitacionAsync(habitacionId));
}
