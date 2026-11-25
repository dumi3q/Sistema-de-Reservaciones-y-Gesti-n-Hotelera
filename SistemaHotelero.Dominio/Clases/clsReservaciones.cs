namespace SistemaHotelero.Dominio.Clases;

// api/Disponibilidad/Buscar. Intervalo [FechaInicio, FechaFin): FechaFin es el día de salida.
public class clsFiltroDisponibilidad
{
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public byte Adultos { get; set; } = 1;
    public byte Ninos { get; set; }
    public int? TipoHabitacionId { get; set; }
    public int? EdificioId { get; set; }
}

// Cuerpo de api/Reservacion/Cotizar y api/Reservacion/Crear
public class clsSolicitudReserva
{
    public int ClienteId { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public string? Observaciones { get; set; }
    public List<clsHabitacionSolicitada> Habitaciones { get; set; } = new();
}

public class clsHabitacionSolicitada
{
    public int HabitacionId { get; set; }
    public byte Adultos { get; set; } = 1;
    public byte Ninos { get; set; }

    // Opcional. Si no se envía, el cliente queda como huésped titular de la primera habitación.
    public List<clsHuespedSolicitado> Huespedes { get; set; } = new();
}

public class clsHuespedSolicitado
{
    public byte? TipoDocumentoId { get; set; }
    public string? NumeroDocumento { get; set; }
    public string Nombres { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public DateOnly? FechaNacimiento { get; set; }
    public bool EsTitular { get; set; }
}

// Filtros opcionales de api/Reservacion/Listar
public class clsFiltroReservacion
{
    public int? ClienteId { get; set; }
    public byte? Estado { get; set; }
    public string? Codigo { get; set; }

    // Reservas cuya estadía se cruza con [Desde, Hasta)
    public DateOnly? Desde { get; set; }
    public DateOnly? Hasta { get; set; }
}

public class clsCancelacion
{
    public string Motivo { get; set; } = null!;
}
