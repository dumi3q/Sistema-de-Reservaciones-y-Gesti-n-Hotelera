using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class Pago
{
    public int Id { get; set; }

    public int CuentaId { get; set; }

    public decimal Monto { get; set; }

    public byte MetodoPago { get; set; }

    public string? Referencia { get; set; }

    public string? UltimosCuatro { get; set; }

    public DateTime FechaPagoUtc { get; set; }

    public byte Estado { get; set; }

    public int UsuarioId { get; set; }

    public Guid ClaveIdempotencia { get; set; }

    public virtual Cuenta Cuenta { get; set; } = null!;

    public virtual ICollection<ReversosPago> ReversosPagos { get; set; } = new List<ReversosPago>();

    public virtual Usuario Usuario { get; set; } = null!;
}
