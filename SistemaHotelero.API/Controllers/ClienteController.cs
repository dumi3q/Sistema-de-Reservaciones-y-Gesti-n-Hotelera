using Microsoft.AspNetCore.Mvc;
using SistemaHotelero.API.Extensiones;
using SistemaHotelero.Dominio.Clases;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.InterfazLN;

namespace SistemaHotelero.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClienteController : ControllerBase
{
    private readonly IClienteLN _clienteLN;

    public ClienteController(IClienteLN clienteLN)
    {
        _clienteLN = clienteLN;
    }

    // GET api/Cliente/Listar?texto=perez&activo=true
    [HttpGet("Listar")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Listar(string? texto = null, bool? activo = null)
        => this.Responder(await _clienteLN.ListarAsync(texto, activo));

    [HttpGet("Obtener/{id:int}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Obtener(int id)
        => this.Responder(await _clienteLN.ObtenerAsync(new TCliente { Id = id }));

    [HttpPost("Crear")]
    public async Task<IActionResult> Crear([FromBody] TCliente datos)
        => this.Responder(await _clienteLN.CrearAsync(datos));

    [HttpPut("Modificar/{id:int}")]
    public async Task<IActionResult> Modificar(int id, [FromBody] TCliente datos)
    {
        datos.Id = id;
        return this.Responder(await _clienteLN.ModificarAsync(datos));
    }

    // Borrado lógico
    [HttpPatch("CambiarActivo/{id:int}")]
    public async Task<IActionResult> CambiarActivo(int id, [FromBody] clsCambioActivo datos)
        => this.Responder(await _clienteLN.CambiarActivoAsync(new TCliente { Id = id, Activo = datos.Activo }));
}
