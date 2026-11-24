using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SolicitudesService.Events;
using SolicitudesService.Interfaces;

namespace SolicitudesService.Consumers
{
    public class SolicitudAtendidaConsumer :
        BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        private IConnection? _connection;
        private IChannel? _channel;

        public SolicitudAtendidaConsumer(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName =
                    _configuration["RabbitMQ:Host"]
                    ?? "localhost",

                Port = int.Parse(
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
                "solicitudes.solicitud-atendida";

            await _channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false);

            await _channel.QueueBindAsync(
                queue: queue,
                exchange: exchange,
                routingKey: "solicitud.atendida");

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false);

            var consumer =
                new AsyncEventingBasicConsumer(
                    _channel);

            consumer.ReceivedAsync +=
                async (_, ea) =>
                {
                    try
                    {
                        var body =
                            ea.Body.ToArray();

                        var json =
                            Encoding.UTF8
                                .GetString(body);

                        var evento =
                            JsonSerializer.Deserialize<
                                SolicitudAtendidaEvent>(
                                    json);

                        if (evento == null)
                        {
                            await _channel
                                .BasicNackAsync(
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
                                    ISolicitudService>();

                        await service.MarcarAtendida(
                            evento.SolicitudId,
                            evento.MovimientoId);

                        await _channel
                            .BasicAckAsync(
                                ea.DeliveryTag,
                                false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            "Error procesando " +
                            $"SolicitudAtendida: " +
                            $"{ex.Message}");

                        await _channel!
                            .BasicNackAsync(
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

            await base.StopAsync(
                cancellationToken);
        }
    }
}