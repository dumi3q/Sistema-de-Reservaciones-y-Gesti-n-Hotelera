namespace SistemaHotelero.Dominio.EntidadesTipadas;

public class THabitacion
{
    public int Id { get; set; }
    public int EdificioId { get; set; }
    public string? EdificioCodigo { get; set; }
    public int TipoHabitacionId { get; set; }
    public string? TipoHabitacionNombre { get; set; }
    public string Numero { get; set; } = null!;
    public byte Piso { get; set; }

    // Ver Enumeraciones.EstadoHabitacion
    public byte Estado { get; set; }
    public string? EstadoNombre { get; set; }

    public List<TAmenidadHabitacion> Amenidades { get; set; } = new();
    public List<TFotoHabitacion> Fotos { get; set; } = new();
}

public class TFotoHabitacion
{
    public int Id { get; set; }
    public int HabitacionId { get; set; }
    public string Url { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int Orden { get; set; }
    public bool EsPrincipal { get; set; }
}

public class THistorialEstadoHabitacion
{
    public int Id { get; set; }
    public int HabitacionId { get; set; }
    public int UsuarioId { get; set; }
    public byte EstadoAnterior { get; set; }
    public byte EstadoNuevo { get; set; }
    public DateTime FechaUtc { get; set; }
    public string? Motivo { get; set; }
}

// Intervalo [FechaInicio, FechaFin): FechaFin es el primer día libre
public class TBloqueoHabitacion
{
    public int Id { get; set; }
    public int HabitacionId { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }

    // Ver Enumeraciones.TipoBloqueo
    public byte Tipo { get; set; }
    public string Motivo { get; set; } = null!;
    public int UsuarioId { get; set; }
    public bool Activo { get; set; }
}
