using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class DetallePedido
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    public int PlatilloId { get; set; }

    public short Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Total { get; set; }

    public string? Instrucciones { get; set; }

    public virtual PedidosRestaurante Pedido { get; set; } = null!;

    public virtual Platillo Platillo { get; set; } = null!;
}
