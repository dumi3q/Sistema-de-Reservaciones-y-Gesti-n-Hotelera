using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class ReservacionHabitacione
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public int HabitacionId { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public byte Adultos { get; set; }

    public byte Ninos { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Total { get; set; }

    public virtual Estadia? Estadia { get; set; }

    public virtual Habitacione Habitacion { get; set; } = null!;

    public virtual ICollection<HuespedesReservacion> HuespedesReservacions { get; set; } = new List<HuespedesReservacion>();

    public virtual Reservacione Reservacion { get; set; } = null!;

    public virtual ICollection<ReservacionNoch> ReservacionNoches { get; set; } = new List<ReservacionNoch>();
}
