using Microsoft.AspNetCore.Mvc;
using SistemaHotelero.Utilitarios;
using System.Security.Claims;

namespace SistemaHotelero.API.Extensiones;

public static class RespuestaHttpExtensiones
{
    // Convierte la Respuesta de la LN en el código HTTP que corresponde
    public static IActionResult Responder<T>(this ControllerBase controlador, Respuesta<T> resultado)
    {
        if (resultado.Success)
            return controlador.Ok(resultado);

        return resultado.Codigo switch
        {
            CodigosRespuesta.NoEncontrado => controlador.NotFound(resultado),
            CodigosRespuesta.Conflicto => controlador.Conflict(resultado),
            CodigosRespuesta.NoPermitido => controlador.StatusCode(StatusCodes.Status403Forbidden, resultado),
            CodigosRespuesta.Error => controlador.StatusCode(StatusCodes.Status500InternalServerError, resultado),
            _ => controlador.BadRequest(resultado)
        };
    }

    // Id del usuario autenticado (claim del JWT). Mientras no exista el login,
    // en Development se usa Desarrollo:UsuarioPruebaId de appsettings.Development.json
    public static int? ObtenerUsuarioId(this ControllerBase controlador)
    {
        var valor = controlador.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? controlador.User.FindFirstValue("sub");

        if (int.TryParse(valor, out var id))
            return id;

        var entorno = controlador.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        if (!entorno.IsDevelopment())
            return null;

        var configuracion = controlador.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        return configuracion.GetValue<int?>("Desarrollo:UsuarioPruebaId");
    }

    public static IActionResult SinUsuario(this ControllerBase controlador)
        => controlador.Unauthorized(Respuesta<object>.Fail("No se identificó al usuario.", CodigosRespuesta.NoPermitido));
}
