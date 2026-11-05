using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class HistorialReservacione
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public int UsuarioId { get; set; }

    public DateTime FechaUtc { get; set; }

    public byte? EstadoAnterior { get; set; }

    public byte EstadoNuevo { get; set; }

    public string? Motivo { get; set; }

    public string? DetalleCambio { get; set; }

    public virtual Reservacione Reservacion { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
