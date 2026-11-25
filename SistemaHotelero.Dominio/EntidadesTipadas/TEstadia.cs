namespace SistemaHotelero.Dominio.EntidadesTipadas;

public class TEstadia
{
    public int Id { get; set; }
    public int ReservacionHabitacionId { get; set; }
    public int ReservacionId { get; set; }
    public string? CodigoReserva { get; set; }
    public int HabitacionId { get; set; }
    public string? Habitacion { get; set; }    // "A-101"
    public int ClienteId { get; set; }
    public string? ClienteNombre { get; set; }
    public int? CuentaId { get; set; }
    public DateTime CheckInRealUtc { get; set; }
    public DateTime? CheckOutRealUtc { get; set; }

    // Ver Enumeraciones.EstadoEstadia
    public byte Estado { get; set; }
}

// Resultado del check-in. El código de acceso solo se muestra esta vez: en la BD queda su hash.
public class TCheckIn
{
    public int ReservacionId { get; set; }
    public string Codigo { get; set; } = null!;
    public List<TAccesoEmitido> Accesos { get; set; } = new();
}

public class TAccesoEmitido
{
    public int EstadiaId { get; set; }
    public string Habitacion { get; set; } = null!;
    public string CodigoAcceso { get; set; } = null!;
    public DateTime ExpiraUtc { get; set; }
}

public class TCheckOut
{
    public int ReservacionId { get; set; }
    public string Codigo { get; set; } = null!;
    public DateTime CheckOutUtc { get; set; }
    public int? CuentaId { get; set; }
    public List<string> HabitacionesEnLimpieza { get; set; } = new();
}
