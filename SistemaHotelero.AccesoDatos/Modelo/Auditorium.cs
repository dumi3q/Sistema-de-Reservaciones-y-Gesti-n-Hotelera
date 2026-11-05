using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class Auditorium
{
    public long Id { get; set; }

    public int? UsuarioId { get; set; }

    public string Accion { get; set; } = null!;

    public string Entidad { get; set; } = null!;

    public string? EntidadId { get; set; }

    public DateTime FechaUtc { get; set; }

    public byte Resultado { get; set; }

    public string? Ip { get; set; }

    public string? DatosNoSensibles { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
