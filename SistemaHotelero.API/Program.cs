using Microsoft.EntityFrameworkCore;
using SistemaHotelero.AccesoDatos.Contexto;
using SistemaHotelero.AccesoDatos.Implementaciones;
using SistemaHotelero.API.Extensiones;
using SistemaHotelero.Dominio.DTO;
using SistemaHotelero.Dominio.InterfazAD;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Base de datos y acceso a datos (compartido por todos los módulos)
builder.Services.AddDbContext<HotelContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("HotelDB")));

builder.Services.AddScoped<IUnidadTrabajoEF, UnidadTrabajoEF>();

// Busca todos los Profile del ensamblado de Dominio (MapeoHospedajeProfile y los demás)
builder.Services.AddAutoMapper(cfg => { }, typeof(MapeoHospedajeProfile).Assembly);

// Módulos de cada compañero
builder.Services.AgregarModulosHospedaje();   // compañero 2: clientes, habitaciones y hospedaje

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
