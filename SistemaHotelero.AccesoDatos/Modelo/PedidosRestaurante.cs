using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class PedidosRestaurante
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public int CuentaId { get; set; }

    public int? EstadiaId { get; set; }

    public int? ReservacionMesaId { get; set; }

    public byte Modalidad { get; set; }

    public string? UbicacionEntrega { get; set; }

    public DateTime FechaPedidoUtc { get; set; }

    public DateTime? FechaHoraDeseadaUtc { get; set; }

    public byte EstadoPedido { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Recargos { get; set; }

    public decimal Total { get; set; }

    public string? Instrucciones { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<CargosCuentum> CargosCuenta { get; set; } = new List<CargosCuentum>();

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual Cuenta Cuenta { get; set; } = null!;

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual Estadia? Estadia { get; set; }

    public virtual ICollection<HistorialPedido> HistorialPedidos { get; set; } = new List<HistorialPedido>();

    public virtual ReservacionesMesa? ReservacionMesa { get; set; }
}
