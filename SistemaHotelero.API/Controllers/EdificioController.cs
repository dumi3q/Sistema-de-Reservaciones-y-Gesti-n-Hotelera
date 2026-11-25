using Microsoft.AspNetCore.Mvc;
using SistemaHotelero.API.Extensiones;
using SistemaHotelero.Dominio.Clases;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.InterfazLN;

namespace SistemaHotelero.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EdificioController : ControllerBase
{
    private readonly IEdificioLN _edificioLN;

    public EdificioController(IEdificioLN edificioLN)
    {
        _edificioLN = edificioLN;
    }

    [HttpGet("Listar")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Listar()
        => this.Responder(await _edificioLN.ListarAsync());

    [HttpGet("Obtener/{id:int}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Obtener(int id)
        => this.Responder(await _edificioLN.ObtenerAsync(new TEdificio { Id = id }));

    [HttpPut("Modificar/{id:int}")]
    public async Task<IActionResult> Modificar(int id, [FromBody] TEdificio datos)
    {
        datos.Id = id;
        return this.Responder(await _edificioLN.ModificarAsync(datos));
    }
}
