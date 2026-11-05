using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class Reservacione
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public int ClienteId { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public byte Estado { get; set; }

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

    public byte[] RowVersion { get; set; } = null!;

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual Usuario CreadoPorUsuario { get; set; } = null!;

    public virtual Cuenta? Cuenta { get; set; }

    public virtual ICollection<HistorialReservacione> HistorialReservaciones { get; set; } = new List<HistorialReservacione>();

    public virtual ICollection<NotificacionesCorreo> NotificacionesCorreos { get; set; } = new List<NotificacionesCorreo>();

    public virtual ICollection<ReservacionHabitacione> ReservacionHabitaciones { get; set; } = new List<ReservacionHabitacione>();
}
