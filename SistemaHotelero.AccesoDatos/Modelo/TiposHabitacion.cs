using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class TiposHabitacion
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public byte CapacidadAdultos { get; set; }

    public byte CapacidadNinos { get; set; }

    public byte CapacidadMaxima { get; set; }

    public byte OcupacionBase { get; set; }

    public decimal PrecioBasePorNoche { get; set; }

    public decimal PrecioPersonaAdicional { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Habitacione> Habitaciones { get; set; } = new List<Habitacione>();

    public virtual ICollection<ReglasTarifa> ReglasTarifas { get; set; } = new List<ReglasTarifa>();
}
