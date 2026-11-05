using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class BloqueosHabitacion
{
    public int Id { get; set; }

    public int HabitacionId { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public byte Tipo { get; set; }

    public string Motivo { get; set; } = null!;

    public int UsuarioId { get; set; }

    public bool Activo { get; set; }

    public virtual Habitacione Habitacion { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
