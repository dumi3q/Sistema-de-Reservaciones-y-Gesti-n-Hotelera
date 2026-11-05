using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class CargosCuentum
{
    public int Id { get; set; }

    public int CuentaId { get; set; }

    public byte Tipo { get; set; }

    public int? EstadiaId { get; set; }

    public int? PedidoId { get; set; }

    public string Concepto { get; set; } = null!;

    public decimal Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Descuento { get; set; }

    public int? ImpuestoId { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Total { get; set; }

    public DateTime FechaUtc { get; set; }

    public int UsuarioId { get; set; }

    public int? CargoRevertidoId { get; set; }

    public string? MotivoAjuste { get; set; }

    public int? AutorizadoPorId { get; set; }

    public virtual Usuario? AutorizadoPor { get; set; }

    public virtual CargosCuentum? CargoRevertido { get; set; }

    public virtual Cuenta Cuenta { get; set; } = null!;

    public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();

    public virtual Estadia? Estadia { get; set; }

    public virtual Impuesto? Impuesto { get; set; }

    public virtual CargosCuentum? InverseCargoRevertido { get; set; }

    public virtual PedidosRestaurante? Pedido { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
