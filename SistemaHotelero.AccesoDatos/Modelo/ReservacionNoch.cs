using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class ReservacionNoch
{
    public int Id { get; set; }

    public int ReservacionHabitacionId { get; set; }

    public int HabitacionId { get; set; }

    public DateOnly Fecha { get; set; }

    public decimal TarifaBase { get; set; }

    public decimal AjusteTarifa { get; set; }

    public decimal TarifaFinal { get; set; }

    public bool Activa { get; set; }

    public virtual Habitacione Habitacion { get; set; } = null!;

    public virtual ReservacionHabitacione ReservacionHabitacion { get; set; } = null!;
}
