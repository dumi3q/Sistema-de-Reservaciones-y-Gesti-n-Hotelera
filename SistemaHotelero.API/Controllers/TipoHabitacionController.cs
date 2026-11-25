using Microsoft.AspNetCore.Mvc;
using SistemaHotelero.API.Extensiones;
using SistemaHotelero.Dominio.Clases;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.InterfazLN;

namespace SistemaHotelero.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TipoHabitacionController : ControllerBase
{
    private readonly ITipoHabitacionLN _tipoLN;

    public TipoHabitacionController(ITipoHabitacionLN tipoLN)
    {
        _tipoLN = tipoLN;
    }

    [HttpGet("Listar")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Listar(bool soloActivos = true)
        => this.Responder(await _tipoLN.ListarAsync(soloActivos));

    [HttpGet("Obtener/{id:int}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Obtener(int id)
        => this.Responder(await _tipoLN.ObtenerAsync(new TTipoHabitacion { Id = id }));

    [HttpPost("Crear")]
    public async Task<IActionResult> Crear([FromBody] TTipoHabitacion datos)
        => this.Responder(await _tipoLN.CrearAsync(datos));

    [HttpPut("Modificar/{id:int}")]
    public async Task<IActionResult> Modificar(int id, [FromBody] TTipoHabitacion datos)
    {
        datos.Id = id;
        return this.Responder(await _tipoLN.ModificarAsync(datos));
    }

    [HttpPatch("CambiarActivo/{id:int}")]
    public async Task<IActionResult> CambiarActivo(int id, [FromBody] clsCambioActivo datos)
        => this.Responder(await _tipoLN.CambiarActivoAsync(new TTipoHabitacion { Id = id, Activo = datos.Activo }));
}
