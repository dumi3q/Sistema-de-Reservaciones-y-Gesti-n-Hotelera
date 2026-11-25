namespace SistemaHotelero.Dominio.EntidadesTipadas;

/* Temporada o recargo de fin de semana.
   Tipo 1 Temporada: usa FechaInicio y FechaFin (ambas incluidas), DiaSemana va null.
   Tipo 2 FinDeSemana: usa DiaSemana (1 lunes ... 7 domingo), las fechas van null.
   TipoAjuste 1 Porcentaje sobre la tarifa base, 2 Monto fijo por noche. Valor negativo = rebaja.
   TipoHabitacionId null = aplica a todos los tipos. Si varias aplican, gana la de mayor Prioridad. */
public class TReglaTarifa
{
    public int Id { get; set; }
    public int? TipoHabitacionId { get; set; }
    public string? TipoHabitacionNombre { get; set; }
    public string Nombre { get; set; } = null!;
    public byte Tipo { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public byte? DiaSemana { get; set; }
    public byte TipoAjuste { get; set; }
    public decimal Valor { get; set; }
    public int Prioridad { get; set; }
    public bool Activo { get; set; }
}
