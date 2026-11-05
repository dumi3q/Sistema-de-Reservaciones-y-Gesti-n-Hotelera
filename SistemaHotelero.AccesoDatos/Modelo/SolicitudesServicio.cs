using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class SolicitudesServicio
{
    public int Id { get; set; }

    public int HabitacionId { get; set; }

    public int? EstadiaId { get; set; }

    public int TipoSolicitudId { get; set; }

    public string Descripcion { get; set; } = null!;

    public byte Prioridad { get; set; }

    public byte Estado { get; set; }

    public int SolicitadoPorUsuarioId { get; set; }

    public int? AsignadoAusuarioId { get; set; }

    public DateTime FechaSolicitudUtc { get; set; }

    public DateTime? FechaPreferidaUtc { get; set; }

    public DateTime? FechaAsignacionUtc { get; set; }

    public DateTime? FechaInicioUtc { get; set; }

    public DateTime? FechaCompletadaUtc { get; set; }

    public string? ComentarioCierre { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Usuario? AsignadoAusuario { get; set; }

    public virtual Estadia? Estadia { get; set; }

    public virtual Habitacione Habitacion { get; set; } = null!;

    public virtual ICollection<HistorialSolicitude> HistorialSolicitudes { get; set; } = new List<HistorialSolicitude>();

    public virtual Usuario SolicitadoPorUsuario { get; set; } = null!;

    public virtual TiposSolicitud TipoSolicitud { get; set; } = null!;
}
