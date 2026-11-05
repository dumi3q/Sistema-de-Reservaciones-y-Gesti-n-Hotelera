using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class Cliente
{
    public int Id { get; set; }

    public byte TipoDocumento { get; set; }

    public string NumeroDocumento { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Nacionalidad { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string Email { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string? Preferencias { get; set; }

    public decimal PorcentajeDescuento { get; set; }

    public DateTime FechaRegistroUtc { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Cuenta> Cuenta { get; set; } = new List<Cuenta>();

    public virtual ICollection<NotificacionesCorreo> NotificacionesCorreos { get; set; } = new List<NotificacionesCorreo>();

    public virtual ICollection<PedidosRestaurante> PedidosRestaurantes { get; set; } = new List<PedidosRestaurante>();

    public virtual ICollection<Reservacione> Reservaciones { get; set; } = new List<Reservacione>();

    public virtual ICollection<ReservacionesMesa> ReservacionesMesas { get; set; } = new List<ReservacionesMesa>();

    public virtual Usuario? Usuario { get; set; }
}
