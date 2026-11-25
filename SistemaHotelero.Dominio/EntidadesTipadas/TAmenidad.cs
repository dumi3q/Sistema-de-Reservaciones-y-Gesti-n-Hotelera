namespace SistemaHotelero.Dominio.EntidadesTipadas;

public class TAmenidad
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}

// Amenidad asignada a una habitación (tabla HabitacionAmenidades)
public class TAmenidadHabitacion
{
    public int AmenidadId { get; set; }
    public string? Nombre { get; set; }
    public byte Cantidad { get; set; }
}
