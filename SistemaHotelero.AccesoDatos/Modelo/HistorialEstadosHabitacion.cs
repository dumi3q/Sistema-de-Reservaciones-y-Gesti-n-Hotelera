using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class HistorialEstadosHabitacion
{
    public int Id { get; set; }

    public int HabitacionId { get; set; }

    public int UsuarioId { get; set; }

    public byte EstadoAnterior { get; set; }

    public byte EstadoNuevo { get; set; }

    public DateTime FechaUtc { get; set; }

    public string? Motivo { get; set; }

    public virtual Habitacione Habitacion { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
