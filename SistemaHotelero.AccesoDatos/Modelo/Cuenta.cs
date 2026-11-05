using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class Cuenta
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public int? ReservacionId { get; set; }

    public byte Tipo { get; set; }

    public byte Estado { get; set; }

    public DateTime FechaAperturaUtc { get; set; }

    public DateTime? FechaCierreUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<CargosCuentum> CargosCuenta { get; set; } = new List<CargosCuentum>();

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual ICollection<PedidosRestaurante> PedidosRestaurantes { get; set; } = new List<PedidosRestaurante>();

    public virtual Reservacione? Reservacion { get; set; }
}
