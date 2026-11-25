using Microsoft.AspNetCore.Mvc;
using SistemaHotelero.API.Extensiones;
using SistemaHotelero.Dominio.Clases;
using SistemaHotelero.Dominio.InterfazLN;

namespace SistemaHotelero.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DisponibilidadController : ControllerBase
{
    private readonly IDisponibilidadLN _disponibilidadLN;

    public DisponibilidadController(IDisponibilidadLN disponibilidadLN)
    {
        _disponibilidadLN = disponibilidadLN;
    }

    // GET api/Disponibilidad/Buscar?fechaInicio=2026-12-10&fechaFin=2026-12-13&adultos=2&ninos=1
    [HttpGet("Buscar")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Buscar([FromQuery] clsFiltroDisponibilidad filtro)
        => this.Responder(await _disponibilidadLN.BuscarAsync(filtro));
}
