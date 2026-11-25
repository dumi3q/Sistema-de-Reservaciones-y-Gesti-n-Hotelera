using System.Text.Json.Serialization;

namespace SistemaHotelero.Utilitarios;

public partial class Respuesta<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Error { get; set; }

    // Ver CodigosRespuesta. La API lo usa para elegir el código HTTP (404, 409...)
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Codigo { get; set; }

    public Respuesta()
    {
        Success = true;
        Error = "";
    }

    protected Respuesta(bool success, T? data, string error, string? codigo = null)
    {
        Success = success;
        Data = data;
        Error = error;
        Codigo = codigo;
    }

    public static Respuesta<T> Ok(T data) => new(true, data, string.Empty);

    public static Respuesta<T> Fail(string error) => new(false, default, error);

    public static Respuesta<T> Fail(string error, string codigo) => new(false, default, error, codigo);
}
