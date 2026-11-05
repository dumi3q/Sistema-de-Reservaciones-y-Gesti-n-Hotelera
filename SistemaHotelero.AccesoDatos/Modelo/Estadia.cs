using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class Estadia
{
    public int Id { get; set; }

    public int ReservacionHabitacionId { get; set; }

    public DateTime CheckInRealUtc { get; set; }

    public DateTime? CheckOutRealUtc { get; set; }

    public int UsuarioCheckInId { get; set; }

    public int? UsuarioCheckOutId { get; set; }

    public byte Estado { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<AccesosEstadium> AccesosEstadia { get; set; } = new List<AccesosEstadium>();

    public virtual ICollection<CargosCuentum> CargosCuenta { get; set; } = new List<CargosCuentum>();

    public virtual ICollection<PedidosRestaurante> PedidosRestaurantes { get; set; } = new List<PedidosRestaurante>();

    public virtual ReservacionHabitacione ReservacionHabitacion { get; set; } = null!;

    public virtual ICollection<SolicitudesServicio> SolicitudesServicios { get; set; } = new List<SolicitudesServicio>();

    public virtual Usuario UsuarioCheckIn { get; set; } = null!;

    public virtual Usuario? UsuarioCheckOut { get; set; }
}
