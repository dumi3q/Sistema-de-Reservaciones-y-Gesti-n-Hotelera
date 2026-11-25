using Microsoft.AspNetCore.Mvc;
using SistemaHotelero.API.Extensiones;
using SistemaHotelero.Dominio.Clases;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.InterfazLN;

namespace SistemaHotelero.API.Controllers;

// Temporadas y recargos de fin de semana
[Route("api/[controller]")]
[ApiController]
public class ReglaTarifaController : ControllerBase
{
    private readonly IReglaTarifaLN _reglaLN;

    public ReglaTarifaController(IReglaTarifaLN reglaLN)
    {
        _reglaLN = reglaLN;
    }

    [HttpGet("Listar")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Listar(bool soloActivos = false)
        => this.Responder(await _reglaLN.ListarAsync(soloActivos));

    [HttpGet("Obtener/{id:int}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Obtener(int id)
        => this.Responder(await _reglaLN.ObtenerAsync(new TReglaTarifa { Id = id }));

    [HttpPost("Crear")]
    public async Task<IActionResult> Crear([FromBody] TReglaTarifa datos)
        => this.Responder(await _reglaLN.CrearAsync(datos));

    [HttpPut("Modificar/{id:int}")]
    public async Task<IActionResult> Modificar(int id, [FromBody] TReglaTarifa datos)
    {
        datos.Id = id;
        return this.Responder(await _reglaLN.ModificarAsync(datos));
    }

    [HttpPatch("CambiarActivo/{id:int}")]
    public async Task<IActionResult> CambiarActivo(int id, [FromBody] clsCambioActivo datos)
        => this.Responder(await _reglaLN.CambiarActivoAsync(new TReglaTarifa { Id = id, Activo = datos.Activo }));
}
