namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class Persona
{
    public int Id { get; set; }

    public byte? TipoDocumentoId { get; set; }

    public string? NumeroDocumento { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string? Nacionalidad { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public string? EmailContacto { get; set; }

    public string? Telefono { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual Usuario? Usuario { get; set; }

    public virtual TipoDocumento? TipoDocumento { get; set; }
}
