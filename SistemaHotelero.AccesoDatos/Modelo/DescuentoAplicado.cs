namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class DescuentoAplicado
{
    public int Id { get; set; }

    public int ReservacionHabitacionId { get; set; }

    public int? DescuentoId { get; set; }

    public int? IndisponibilidadId { get; set; }

    public int? HabitacionId { get; set; }

    public int? AmenidadId { get; set; }

    public decimal Monto { get; set; }

    public string Motivo { get; set; } = null!;

    public int UsuarioId { get; set; }

    public DateTime FechaUtc { get; set; }
}
