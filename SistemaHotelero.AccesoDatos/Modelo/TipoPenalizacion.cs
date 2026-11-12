namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class TipoPenalizacion
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal? MontoReferencia { get; set; }

    public bool Activo { get; set; }
}
