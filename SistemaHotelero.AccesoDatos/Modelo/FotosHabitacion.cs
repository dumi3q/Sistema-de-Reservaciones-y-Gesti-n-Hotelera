using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class FotosHabitacion
{
    public int Id { get; set; }

    public int HabitacionId { get; set; }

    public string Url { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int Orden { get; set; }

    public bool EsPrincipal { get; set; }

    public virtual Habitacione Habitacion { get; set; } = null!;
}
