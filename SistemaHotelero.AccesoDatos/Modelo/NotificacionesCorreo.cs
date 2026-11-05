using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class NotificacionesCorreo
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public byte Tipo { get; set; }

    public int? ReservacionId { get; set; }

    public int? FacturaId { get; set; }

    public string Destinatario { get; set; } = null!;

    public byte Estado { get; set; }

    public int Intentos { get; set; }

    public string? UltimoError { get; set; }

    public DateTime FechaCreacionUtc { get; set; }

    public DateTime? FechaEnvioUtc { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual Factura? Factura { get; set; }

    public virtual Reservacione? Reservacion { get; set; }
}
