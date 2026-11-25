using SistemaHotelero.API.Servicios;
using SistemaHotelero.Dominio.InterfazLN;
using SistemaHotelero.LogicaNegocio.Implementacion;

namespace SistemaHotelero.API.Extensiones;

// Dependencias de los módulos de hospedaje (compañero 2).
// Se llama desde Program.cs con builder.Services.AgregarModulosHospedaje();
public static class RegistroHospedaje
{
    public static IServiceCollection AgregarModulosHospedaje(this IServiceCollection services)
    {
        // Clientes y habitaciones
        services.AddScoped<IClienteLN, ClienteLN>();
        services.AddScoped<IEdificioLN, EdificioLN>();
        services.AddScoped<ITipoHabitacionLN, TipoHabitacionLN>();
        services.AddScoped<IAmenidadLN, AmenidadLN>();
        services.AddScoped<IHabitacionLN, HabitacionLN>();
        services.AddScoped<IBloqueoHabitacionLN, BloqueoHabitacionLN>();
        services.AddScoped<IReglaTarifaLN, ReglaTarifaLN>();

        // Reservas y estadías
        services.AddScoped<IDisponibilidadLN, DisponibilidadLN>();
        services.AddScoped<IReservacionLN, ReservacionLN>();
        services.AddScoped<IEstadiaLN, EstadiaLN>();

        // Vence las reservas sin anticipo en segundo plano
        services.AddHostedService<VencimientoReservasServicio>();

        return services;
    }
}
