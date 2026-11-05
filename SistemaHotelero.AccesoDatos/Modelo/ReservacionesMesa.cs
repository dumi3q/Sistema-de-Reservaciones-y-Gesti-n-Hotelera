using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class ReservacionesMesa
{
    public int Id { get; set; }

    public int MesaId { get; set; }

    public int ClienteId { get; set; }

    public DateTime FechaInicioUtc { get; set; }

    public DateTime FechaFinUtc { get; set; }

    public byte CantidadPersonas { get; set; }

    public byte Estado { get; set; }

    public string? Observaciones { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual Mesa Mesa { get; set; } = null!;

    public virtual ICollection<PedidosRestaurante> PedidosRestaurantes { get; set; } = new List<PedidosRestaurante>();
}
