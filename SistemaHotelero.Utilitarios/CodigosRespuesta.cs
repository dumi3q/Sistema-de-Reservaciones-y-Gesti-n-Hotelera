namespace SistemaHotelero.Utilitarios;

// Códigos que viajan en Respuesta.Codigo. La API los traduce a HTTP:
// Validacion 400, NoEncontrado 404, Conflicto 409, NoPermitido 403, Error 500
public static class CodigosRespuesta
{
    public const string Validacion = "VALIDACION";
    public const string NoEncontrado = "NO_ENCONTRADO";
    public const string Conflicto = "CONFLICTO";
    public const string NoPermitido = "NO_PERMITIDO";
    public const string Error = "ERROR";
}
