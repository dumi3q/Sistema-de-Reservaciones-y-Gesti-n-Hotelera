using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class Factura
{
    public int Id { get; set; }

    public int CuentaId { get; set; }

    public string Numero { get; set; } = null!;

    public DateTime FechaEmisionUtc { get; set; }

    public string NombreReceptor { get; set; } = null!;

    public string DocumentoReceptor { get; set; } = null!;

    public string EmailReceptor { get; set; } = null!;

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Total { get; set; }

    public byte Estado { get; set; }

    public virtual Cuenta Cuenta { get; set; } = null!;

    public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();

    public virtual ICollection<NotificacionesCorreo> NotificacionesCorreos { get; set; } = new List<NotificacionesCorreo>();
}
