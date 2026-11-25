using SistemaHotelero.Dominio.InterfazLN;

namespace SistemaHotelero.API.Servicios;

// Cada 15 minutos cancela las reservas PendientePago cuyo plazo para pagar el anticipo venció
// (Configuracion.HorasExpiracionPendiente) y libera sus noches.
public class VencimientoReservasServicio : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(15);

    private readonly IServiceScopeFactory _fabricaAlcances;
    private readonly ILogger<VencimientoReservasServicio> _logger;

    public VencimientoReservasServicio(
        IServiceScopeFactory fabricaAlcances, ILogger<VencimientoReservasServicio> logger)
    {
        _fabricaAlcances = fabricaAlcances;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var temporizador = new PeriodicTimer(Intervalo);

        do
        {
            try
            {
                // Las LN son scoped (usan el DbContext): se crea un alcance por vuelta
                using var alcance = _fabricaAlcances.CreateScope();
                var reservacionLN = alcance.ServiceProvider.GetRequiredService<IReservacionLN>();

                var resultado = await reservacionLN.VencerPendientesAsync();
                if (resultado.Success && resultado.Data > 0)
                    _logger.LogInformation("Se vencieron {Cantidad} reservas sin anticipo.", resultado.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al vencer reservas pendientes.");
            }
        }
        while (await temporizador.WaitForNextTickAsync(stoppingToken));
    }
}
