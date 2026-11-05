using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class TiposSolicitud
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public byte Area { get; set; }

    public short TiempoObjetivoMinutos { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<SolicitudesServicio> SolicitudesServicios { get; set; } = new List<SolicitudesServicio>();
}
