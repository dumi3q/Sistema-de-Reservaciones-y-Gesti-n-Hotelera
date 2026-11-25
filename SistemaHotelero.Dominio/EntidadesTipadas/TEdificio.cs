namespace SistemaHotelero.Dominio.EntidadesTipadas;

public class TEdificio
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public byte CantidadPisos { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}
