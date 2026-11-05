using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class ReglasTarifa
{
    public int Id { get; set; }

    public int? TipoHabitacionId { get; set; }

    public string Nombre { get; set; } = null!;

    public byte Tipo { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public byte? DiaSemana { get; set; }

    public byte TipoAjuste { get; set; }

    public decimal Valor { get; set; }

    public int Prioridad { get; set; }

    public bool Activo { get; set; }

    public virtual TiposHabitacion? TipoHabitacion { get; set; }
}
