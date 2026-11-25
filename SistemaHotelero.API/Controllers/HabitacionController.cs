using Microsoft.AspNetCore.Mvc;
using SistemaHotelero.API.Extensiones;
using SistemaHotelero.Dominio.Clases;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.InterfazLN;

namespace SistemaHotelero.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HabitacionController : ControllerBase
{
    private readonly IHabitacionLN _habitacionLN;
    private readonly IBloqueoHabitacionLN _bloqueoLN;

    public HabitacionController(IHabitacionLN habitacionLN, IBloqueoHabitacionLN bloqueoLN)
    {
        _habitacionLN = habitacionLN;
        _bloqueoLN = bloqueoLN;
    }

    // GET api/Habitacion/Listar?edificioId=1&piso=2&tipoHabitacionId=3&estado=1
    [HttpGet("Listar")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Listar([FromQuery] clsFiltroHabitacion filtro)
        => this.Responder(await _habitacionLN.ListarAsync(filtro));

    // Incluye amenidades y fotos
    [HttpGet("Obtener/{id:int}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Obtener(int id)
        => this.Responder(await _habitacionLN.ObtenerAsync(new THabitacion { Id = id }));

    [HttpPost("Crear")]
    public async Task<IActionResult> Crear([FromBody] THabitacion datos)
        => this.Responder(await _habitacionLN.CrearAsync(datos));

    [HttpPut("Modificar/{id:int}")]
    public async Task<IActionResult> Modificar(int id, [FromBody] THabitacion datos)
    {
        datos.Id = id;
        return this.Responder(await _habitacionLN.ModificarAsync(datos));
    }

    #region Estado operativo

    [HttpPatch("CambiarEstado/{id:int}")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] clsCambioEstadoHabitacion datos)
    {
        var usuarioId = this.ObtenerUsuarioId();
        if (usuarioId == null)
            return this.SinUsuario();

        return this.Responder(await _habitacionLN.CambiarEstadoAsync(id, datos, usuarioId.Value));
    }

    [HttpGet("HistorialEstados/{id:int}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> HistorialEstados(int id)
        => this.Responder(await _habitacionLN.ListarHistorialEstadosAsync(id));

    #endregion

    #region Amenidades y fotos

    // Reemplaza la lista completa: lo que no venga en el cuerpo se quita
    [HttpPut("Amenidades/{id:int}")]
    public async Task<IActionResult> AsignarAmenidades(int id, [FromBody] List<clsAmenidadAsignada> amenidades)
        => this.Responder(await _habitacionLN.AsignarAmenidadesAsync(id, amenidades));

    [HttpPost("Fotos/{id:int}")]
    public async Task<IActionResult> AgregarFoto(int id, [FromBody] TFotoHabitacion datos)
    {
        datos.HabitacionId = id;
        return this.Responder(await _habitacionLN.AgregarFotoAsync(datos));
    }

    [HttpDelete("Fotos/{fotoId:int}")]
    public async Task<IActionResult> EliminarFoto(int fotoId)
        => this.Responder(await _habitacionLN.EliminarFotoAsync(fotoId));

    [HttpPatch("Fotos/{fotoId:int}/Principal")]
    public async Task<IActionResult> MarcarFotoPrincipal(int fotoId)
        => this.Responder(await _habitacionLN.MarcarFotoPrincipalAsync(fotoId));

    #endregion

    #region Bloqueos por fechas

    // GET api/Habitacion/Bloqueos?habitacionId=5&soloActivos=true
    [HttpGet("Bloqueos")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> ListarBloqueos(int? habitacionId = null, bool soloActivos = true)
        => this.Responder(await _bloqueoLN.ListarAsync(habitacionId, soloActivos));

    [HttpPost("Bloqueos/{id:int}")]
    public async Task<IActionResult> CrearBloqueo(int id, [FromBody] TBloqueoHabitacion datos)
    {
        var usuarioId = this.ObtenerUsuarioId();
        if (usuarioId == null)
            return this.SinUsuario();

        datos.HabitacionId = id;
        return this.Responder(await _bloqueoLN.CrearAsync(datos, usuarioId.Value));
    }

    [HttpPatch("Bloqueos/{bloqueoId:int}/Desactivar")]
    public async Task<IActionResult> DesactivarBloqueo(int bloqueoId)
        => this.Responder(await _bloqueoLN.DesactivarAsync(bloqueoId));

    #endregion
}
