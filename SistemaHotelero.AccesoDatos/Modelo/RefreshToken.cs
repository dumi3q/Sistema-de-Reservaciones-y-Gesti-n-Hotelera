using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class RefreshToken
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string TokenHash { get; set; } = null!;

    public Guid FamiliaId { get; set; }

    public DateTime FechaCreacionUtc { get; set; }

    public DateTime FechaExpiracionUtc { get; set; }

    public DateTime? FechaUsoUtc { get; set; }

    public DateTime? FechaRevocacionUtc { get; set; }

    public string? MotivoRevocacion { get; set; }

    public int? ReemplazadoPorId { get; set; }

    public virtual ICollection<RefreshToken> InverseReemplazadoPor { get; set; } = new List<RefreshToken>();

    public virtual RefreshToken? ReemplazadoPor { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
