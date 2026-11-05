using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class AccesosEstadium
{
    public int Id { get; set; }

    public int EstadiaId { get; set; }

    public string IdentificadorAcceso { get; set; } = null!;

    public byte Tipo { get; set; }

    public DateTime FechaEmisionUtc { get; set; }

    public DateTime FechaExpiracionUtc { get; set; }

    public DateTime? FechaRevocacionUtc { get; set; }

    public virtual Estadia Estadia { get; set; } = null!;
}
