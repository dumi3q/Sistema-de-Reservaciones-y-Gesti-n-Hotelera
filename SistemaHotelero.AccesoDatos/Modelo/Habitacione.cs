using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class Habitacione
{
    public int Id { get; set; }

    public int EdificioId { get; set; }

    public int TipoHabitacionId { get; set; }

    public string Numero { get; set; } = null!;

    public byte Piso { get; set; }

    public byte Estado { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<BloqueosHabitacion> BloqueosHabitacions { get; set; } = new List<BloqueosHabitacion>();

    public virtual Edificio Edificio { get; set; } = null!;

    public virtual FotosHabitacion? FotosHabitacion { get; set; }

    public virtual ICollection<HabitacionAmenidade> HabitacionAmenidades { get; set; } = new List<HabitacionAmenidade>();

    public virtual ICollection<HistorialEstadosHabitacion> HistorialEstadosHabitacions { get; set; } = new List<HistorialEstadosHabitacion>();

    public virtual ICollection<ReservacionHabitacione> ReservacionHabitaciones { get; set; } = new List<ReservacionHabitacione>();

    public virtual ICollection<SolicitudesServicio> SolicitudesServicios { get; set; } = new List<SolicitudesServicio>();

    public virtual TiposHabitacion TipoHabitacion { get; set; } = null!;
}
