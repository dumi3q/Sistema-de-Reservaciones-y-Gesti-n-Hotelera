using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class Usuario
{
    public int Id { get; set; }

    public int PersonaId { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool Activo { get; set; }

    public int IntentosFallidos { get; set; }

    public DateTime? BloqueadoHastaUtc { get; set; }

    public DateTime? UltimoAccesoUtc { get; set; }

    public DateTime FechaCreacionUtc { get; set; }

    public virtual ICollection<Auditorium> Auditoria { get; set; } = new List<Auditorium>();

    public virtual ICollection<BloqueosHabitacion> BloqueosHabitacions { get; set; } = new List<BloqueosHabitacion>();

    public virtual ICollection<CargosCuentum> CargosCuentumAutorizadoPors { get; set; } = new List<CargosCuentum>();

    public virtual ICollection<CargosCuentum> CargosCuentumUsuarios { get; set; } = new List<CargosCuentum>();

    public virtual Persona Persona { get; set; } = null!;

    public virtual ICollection<Estadia> EstadiaUsuarioCheckIns { get; set; } = new List<Estadia>();

    public virtual ICollection<Estadia> EstadiaUsuarioCheckOuts { get; set; } = new List<Estadia>();

    public virtual ICollection<HistorialEstadosHabitacion> HistorialEstadosHabitacions { get; set; } = new List<HistorialEstadosHabitacion>();

    public virtual ICollection<HistorialPedido> HistorialPedidos { get; set; } = new List<HistorialPedido>();

    public virtual ICollection<HistorialReservacione> HistorialReservaciones { get; set; } = new List<HistorialReservacione>();

    public virtual ICollection<HistorialSolicitude> HistorialSolicitudes { get; set; } = new List<HistorialSolicitude>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<Reservacione> Reservaciones { get; set; } = new List<Reservacione>();

    public virtual ICollection<ReversosPago> ReversosPagos { get; set; } = new List<ReversosPago>();

    public virtual ICollection<SolicitudesServicio> SolicitudesServicioAsignadoAusuarios { get; set; } = new List<SolicitudesServicio>();

    public virtual ICollection<SolicitudesServicio> SolicitudesServicioSolicitadoPorUsuarios { get; set; } = new List<SolicitudesServicio>();

    public virtual ICollection<Role> Rols { get; set; } = new List<Role>();
}
