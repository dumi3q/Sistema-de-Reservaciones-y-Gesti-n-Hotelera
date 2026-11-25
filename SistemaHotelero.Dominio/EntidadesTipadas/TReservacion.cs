namespace SistemaHotelero.Dominio.EntidadesTipadas;

public class TReservacion
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public int ClienteId { get; set; }
    public string? ClienteNombre { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public int Noches { get; set; }

    // Ver Enumeraciones.EstadoReservacion
    public byte Estado { get; set; }
    public string? EstadoNombre { get; set; }

    public byte CantidadAdultos { get; set; }
    public byte CantidadNinos { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Total { get; set; }
    public decimal AnticipoRequerido { get; set; }
    public DateTime? FechaExpiracionPendienteUtc { get; set; }
    public string? Observaciones { get; set; }
    public string? MotivoCancelacion { get; set; }
    public int CreadoPorUsuarioId { get; set; }
    public DateTime FechaCreacionUtc { get; set; }

    // Cuenta de hospedaje (los pagos del anticipo y del saldo se registran en ella)
    public int? CuentaId { get; set; }
    public decimal? MontoPagado { get; set; }
    public decimal? Saldo { get; set; }

    public List<TReservacionHabitacion> Habitaciones { get; set; } = new();
}

public class TReservacionHabitacion
{
    public int Id { get; set; }
    public int HabitacionId { get; set; }
    public string? Habitacion { get; set; }          // "A-101"
    public string? TipoHabitacionNombre { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public byte Adultos { get; set; }
    public byte Ninos { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Total { get; set; }

    public int? EstadiaId { get; set; }
    public byte? EstadoEstadia { get; set; }

    public List<TReservacionNoche> Noches { get; set; } = new();
    public List<THuespedReservacion> Huespedes { get; set; } = new();
}

public class TReservacionNoche
{
    public DateOnly Fecha { get; set; }
    public decimal TarifaBase { get; set; }
    public decimal AjusteTarifa { get; set; }
    public decimal TarifaFinal { get; set; }
    public string? ReglaAplicada { get; set; }
}

public class THuespedReservacion
{
    public int Id { get; set; }
    public byte? TipoDocumentoId { get; set; }
    public string? NumeroDocumento { get; set; }
    public string Nombres { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public DateOnly? FechaNacimiento { get; set; }
    public bool EsTitular { get; set; }
}

public class THistorialReservacion
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public DateTime FechaUtc { get; set; }
    public byte? EstadoAnterior { get; set; }
    public byte EstadoNuevo { get; set; }
    public string? Motivo { get; set; }
}

// Resultado de api/Reservacion/Cotizar: lo mismo que se guardaría al crear
public class TCotizacion
{
    public int ClienteId { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public int Noches { get; set; }
    public decimal PorcentajeDescuentoCliente { get; set; }
    public decimal PorcentajeImpuesto { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Total { get; set; }
    public decimal AnticipoRequerido { get; set; }
    public List<TReservacionHabitacion> Habitaciones { get; set; } = new();
}

// Resultado de api/Disponibilidad/Buscar
public class TDisponibilidadHabitacion
{
    public int HabitacionId { get; set; }
    public int EdificioId { get; set; }
    public string Habitacion { get; set; } = null!;   // "A-101"
    public byte Piso { get; set; }
    public int TipoHabitacionId { get; set; }
    public string TipoHabitacionNombre { get; set; } = null!;
    public byte CapacidadMaxima { get; set; }
    public int Noches { get; set; }

    // Antes de descuento del cliente e impuestos
    public decimal PrecioPromedioNoche { get; set; }
    public decimal SubtotalEstimado { get; set; }
}
