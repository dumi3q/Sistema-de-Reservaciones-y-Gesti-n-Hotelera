using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class HuespedesReservacion
{
    public int Id { get; set; }

    public int ReservacionHabitacionId { get; set; }

    public byte? TipoDocumentoId { get; set; }

    public virtual TipoDocumento? TipoDocumento { get; set; }

    public string? NumeroDocumento { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public DateOnly? FechaNacimiento { get; set; }

    public bool EsTitular { get; set; }

    public virtual ReservacionHabitacione ReservacionHabitacion { get; set; } = null!;
}
