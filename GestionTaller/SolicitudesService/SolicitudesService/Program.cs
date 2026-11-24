using Microsoft.EntityFrameworkCore;
using SolicitudesService.Data;
using SolicitudesService.Interfaces;
using SolicitudesService.Services;
using SolicitudesService.Messaging;
using SolicitudesService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<SolicitudesDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "SolicitudesConnection")
    )
);

builder.Services.AddScoped<ISolicitudService,SolicitudService>();
builder.Services.AddSingleton<IRabbitMqPublisher,RabbitMqPublisher>();
builder.Services.AddHostedService<SolicitudAtendidaConsumer>();
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