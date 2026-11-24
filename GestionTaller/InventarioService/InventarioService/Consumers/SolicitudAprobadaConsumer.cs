using System.Text;
using System.Text.Json;
using InventarioService.DTOs;
using InventarioService.Events;
using InventarioService.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using InventarioService.Messaging;

namespace InventarioService.Consumers
{
    public class SolicitudAprobadaConsumer :
        BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IRabbitMqPublisher _publisher;

        private IConnection? _connection;
        private IChannel? _channel;

        public SolicitudAprobadaConsumer(
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory,
    IRabbitMqPublisher publisher)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _publisher = publisher;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName =
                    _configuration["RabbitMQ:Host"]
                    ?? "localhost",

                Port =
                    int.Parse(
                        _configuration["RabbitMQ:Port"]
                        ?? "5672"),

                UserName =
                    _configuration["RabbitMQ:Username"],

                Password =
                    _configuration["RabbitMQ:Password"],

                AutomaticRecoveryEnabled = true
            };

            _connection =
                await factory.CreateConnectionAsync();

            _channel =
                await _connection.CreateChannelAsync();

            var exchange =
                _configuration["RabbitMQ:Exchange"]
                ?? "gestiontaller.events";

            await _channel.ExchangeDeclareAsync(
                exchange: exchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            const string queue =
                "inventario.solicitud-aprobada";

            await _channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false);

            await _channel.QueueBindAsync(
                queue: queue,
                exchange: exchange,
                routingKey: "solicitud.aprobada");

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false);

            var consumer =
                new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync +=
                async (_, ea) =>
                {
                    try
                    {
                        var body =
                            ea.Body.ToArray();

                        var json =
                            Encoding.UTF8.GetString(body);

                        var evento =
                            JsonSerializer.Deserialize<
                                SolicitudAprobadaEvent>(
                                    json);

                        if (evento == null)
                        {
                            await _channel.BasicNackAsync(
                                ea.DeliveryTag,
                                false,
                                false);

                            return;
                        }

                        using var scope =
                            _scopeFactory.CreateScope();

                        var service =
                            scope.ServiceProvider
                                .GetRequiredService<
                                    IInventarioService>();

                        var salida =
                            new SalidaDto
                            {
                                BodegaOrigenId =
                                    evento.BodegaId,

                                UsuarioId =
                                    evento.UsuarioAprobadorId,

                                SolicitudId =
                                    evento.SolicitudId,

                                Observacion =
                                    $"Salida generada por " +
                                    $"solicitud " +
                                    $"{evento.SolicitudId}.",

                                Detalles =
                                    evento.Detalles
                                        .Select(x =>
                                            new MovimientoDetalleRequestDto
                                            {
                                                MaterialId =
                                                    x.MaterialId,

                                                Cantidad =
                                                    x.Cantidad
                                            })
                                        .ToList()
                            };

                        var resultado =
                            await service.RegistrarSalida(salida);

                        var eventoAtendido =
                            new SolicitudAtendidaEvent
                            {
                                SolicitudId =
                                    evento.SolicitudId,

                                MovimientoId =
                                    resultado.MovimientoId,

                                Fecha =
                                    DateTime.Now,

                                Estado =
                                    "ATENDIDA"
                            };

                        await _publisher.PublicarAsync(
                            "solicitud.atendida",
                            eventoAtendido);

                        await _channel.BasicAckAsync(
                            ea.DeliveryTag,
                            false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"Error procesando solicitud: " +
                            $"{ex.Message}");

                        await _channel!.BasicNackAsync(
                            ea.DeliveryTag,
                            false,
                            false);
                    }
                };

            await _channel.BasicConsumeAsync(
                queue: queue,
                autoAck: false,
                consumer: consumer);

            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }

        public override async Task StopAsync(
            CancellationToken cancellationToken)
        {
            if (_channel != null)
                await _channel.DisposeAsync();

            if (_connection != null)
                await _connection.DisposeAsync();

            await base.StopAsync(cancellationToken);
        }
    }
}