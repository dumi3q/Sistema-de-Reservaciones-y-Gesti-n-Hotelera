using System;
using System.Collections.Generic;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class Impuesto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public byte Ambito { get; set; }

    public decimal Porcentaje { get; set; }

    public DateOnly VigenteDesde { get; set; }

    public DateOnly? VigenteHasta { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<CargosCuentum> CargosCuenta { get; set; } = new List<CargosCuentum>();
}
