namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class Descuento
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public byte TipoCalculo { get; set; }

    public decimal Valor { get; set; }

    public DateOnly VigenteDesde { get; set; }

    public DateOnly? VigenteHasta { get; set; }

    public bool Activo { get; set; }
}
