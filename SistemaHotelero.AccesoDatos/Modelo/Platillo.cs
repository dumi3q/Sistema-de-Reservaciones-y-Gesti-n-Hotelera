using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class Platillo
{
    public int Id { get; set; }

    public int CategoriaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public decimal Precio { get; set; }

    public string? ImagenUrl { get; set; }

    public bool Disponible { get; set; }

    public short TiempoPreparacionMinutos { get; set; }

    public bool Activo { get; set; }

    public virtual CategoriasPlatillo Categoria { get; set; } = null!;

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
}
