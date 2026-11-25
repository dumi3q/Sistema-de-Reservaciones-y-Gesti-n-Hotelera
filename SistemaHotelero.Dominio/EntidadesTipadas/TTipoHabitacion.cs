namespace SistemaHotelero.Dominio.EntidadesTipadas;

public class TTipoHabitacion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public byte CapacidadAdultos { get; set; }
    public byte CapacidadNinos { get; set; }
    public byte CapacidadMaxima { get; set; }
    public byte OcupacionBase { get; set; }
    public decimal PrecioBasePorNoche { get; set; }
    public decimal PrecioPersonaAdicional { get; set; }
    public bool Activo { get; set; }
}
