namespace SistemaHotelero.Dominio.EntidadesTipadas;

// Cliente + sus datos personales (tabla Personas) en un solo objeto plano
public class TCliente
{
    public int Id { get; set; }
    public int PersonaId { get; set; }

    public byte? TipoDocumentoId { get; set; }
    public string? TipoDocumentoNombre { get; set; }
    public string? NumeroDocumento { get; set; }
    public string Nombres { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public string? Nacionalidad { get; set; }
    public DateOnly? FechaNacimiento { get; set; }
    public string? EmailContacto { get; set; }
    public string? Telefono { get; set; }

    public string? Preferencias { get; set; }
    public decimal PorcentajeDescuento { get; set; }
    public DateTime FechaRegistroUtc { get; set; }
    public bool Activo { get; set; }
}
