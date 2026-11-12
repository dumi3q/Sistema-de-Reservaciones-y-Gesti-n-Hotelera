namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class IndisponibilidadAmenidad
{
    public int Id { get; set; }

    public int HabitacionId { get; set; }

    public int AmenidadId { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public string Motivo { get; set; } = null!;

    public int UsuarioId { get; set; }

    public bool Activo { get; set; }
}
