namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class CompensacionAmenidadPendiente
{
    public int ReservacionHabitacionId { get; set; }

    public int IndisponibilidadId { get; set; }

    public int HabitacionId { get; set; }

    public int AmenidadId { get; set; }

    public int? NochesAfectadas { get; set; }

    public decimal? BaseAfectada { get; set; }
}
