using Microsoft.AspNetCore.Mvc;
using SistemaHotelero.API.Extensiones;
using SistemaHotelero.Dominio.Clases;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.InterfazLN;

namespace SistemaHotelero.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AmenidadController : ControllerBase
{
    private readonly IAmenidadLN _amenidadLN;

    public AmenidadController(IAmenidadLN amenidadLN)
    {
        _amenidadLN = amenidadLN;
    }

    [HttpGet("Listar")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Listar(bool soloActivos = true)
        => this.Responder(await _amenidadLN.ListarAsync(soloActivos));

    [HttpGet("Obtener/{id:int}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Obtener(int id)
        => this.Responder(await _amenidadLN.ObtenerAsync(new TAmenidad { Id = id }));

    [HttpPost("Crear")]
    public async Task<IActionResult> Crear([FromBody] TAmenidad datos)
        => this.Responder(await _amenidadLN.CrearAsync(datos));

    [HttpPut("Modificar/{id:int}")]
    public async Task<IActionResult> Modificar(int id, [FromBody] TAmenidad datos)
    {
        datos.Id = id;
        return this.Responder(await _amenidadLN.ModificarAsync(datos));
    }

    [HttpPatch("CambiarActivo/{id:int}")]
    public async Task<IActionResult> CambiarActivo(int id, [FromBody] clsCambioActivo datos)
        => this.Responder(await _amenidadLN.CambiarActivoAsync(new TAmenidad { Id = id, Activo = datos.Activo }));
}
