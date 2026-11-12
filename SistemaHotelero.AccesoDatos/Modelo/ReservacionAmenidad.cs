namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class ReservacionAmenidad
{
    public int ReservacionHabitacionId { get; set; }

    public int HabitacionId { get; set; }

    public int AmenidadId { get; set; }

    public byte Cantidad { get; set; }

    public DateTime FechaRegistroUtc { get; set; }
}
