using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class HistorialSolicitude
{
    public int Id { get; set; }

    public int SolicitudId { get; set; }

    public int UsuarioId { get; set; }

    public byte? EstadoAnterior { get; set; }

    public byte EstadoNuevo { get; set; }

    public DateTime FechaUtc { get; set; }

    public string? Comentario { get; set; }

    public virtual SolicitudesServicio Solicitud { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
