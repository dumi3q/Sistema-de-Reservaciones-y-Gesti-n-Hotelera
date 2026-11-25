using System;
using System.Collections.Generic;

namespace SistemaHotelero.Dominio.Entidades;

public partial class ReversosPago
{
    public int Id { get; set; }

    public int PagoId { get; set; }

    public decimal Monto { get; set; }

    public string Motivo { get; set; } = null!;

    public DateTime FechaUtc { get; set; }

    public int UsuarioId { get; set; }

    public virtual Pago Pago { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
