using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class HabitacionAmenidade
{
    public int HabitacionId { get; set; }

    public int AmenidadId { get; set; }

    public byte Cantidad { get; set; }

    public virtual Amenidade Amenidad { get; set; } = null!;

    public virtual Habitacione Habitacion { get; set; } = null!;
}
