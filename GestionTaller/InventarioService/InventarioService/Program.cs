using InventarioService.Data;
using Microsoft.EntityFrameworkCore;
using InventarioService.Interfaces;
using InventarioService.Services;
using InventarioService.Consumers;
using InventarioService.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Entity Framework + SQL Server
builder.Services.AddDbContext<InventarioDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("InventarioConnection")
    )
);

builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IBodegaService, BodegaService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IInventarioService, GestionInventarioService>();
builder.Services.AddScoped<FlyweightBenchmarkService>();

// Patrones de comportamiento: Command + Observer
builder.Services.AddScoped<InventarioService.Patterns.Command.CommandInvoker>();
builder.Services.AddScoped<InventarioService.Patterns.Observer.IMaterialCreadoObserver, InventarioService.Patterns.Observer.AuditoriaMaterialObserver>();
builder.Services.AddScoped<InventarioService.Patterns.Observer.IMaterialCreadoObserver, InventarioService.Patterns.Observer.LogMaterialObserver>();
builder.Services.AddScoped<InventarioService.Patterns.Observer.MaterialCreadoSubject>();
builder.Services.AddHostedService<SolicitudAprobadaConsumer>();
builder.Services.AddSingleton<IRabbitMqPublisher,RabbitMqPublisher>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();