using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class DetalleFactura
{
    public int Id { get; set; }

    public int FacturaId { get; set; }

    public int CargoCuentaId { get; set; }

    public string Descripcion { get; set; } = null!;

    public decimal Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Total { get; set; }

    public virtual CargosCuentum CargoCuenta { get; set; } = null!;

    public virtual Factura Factura { get; set; } = null!;
}
