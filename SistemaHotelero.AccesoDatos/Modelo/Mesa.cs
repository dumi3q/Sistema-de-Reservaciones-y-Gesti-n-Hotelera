using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class Mesa
{
    public int Id { get; set; }

    public string Numero { get; set; } = null!;

    public byte Capacidad { get; set; }

    public byte Ubicacion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<ReservacionesMesa> ReservacionesMesas { get; set; } = new List<ReservacionesMesa>();
}
