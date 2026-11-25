namespace SistemaHotelero.Dominio.Clases;

// Filtros opcionales para api/Habitacion/Listar
public class clsFiltroHabitacion
{
    public int? EdificioId { get; set; }
    public byte? Piso { get; set; }
    public int? TipoHabitacionId { get; set; }
    public byte? Estado { get; set; }
}

public class clsCambioEstadoHabitacion
{
    public byte EstadoNuevo { get; set; }
    public string? Motivo { get; set; }
}

public class clsAmenidadAsignada
{
    public int AmenidadId { get; set; }
    public byte Cantidad { get; set; } = 1;
}

public class clsCambioActivo
{
    public bool Activo { get; set; }
}
