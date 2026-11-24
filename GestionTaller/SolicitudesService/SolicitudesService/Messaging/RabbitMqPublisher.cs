using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace SolicitudesService.Messaging
{
    public class RabbitMqPublisher :
        IRabbitMqPublisher,
        IAsyncDisposable
    {
        private readonly IConfiguration _configuration;

        private IConnection? _connection;
        private IChannel? _channel;

        private readonly SemaphoreSlim _lock = new(1, 1);

        public RabbitMqPublisher(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task InicializarAsync()
        {
            if (_connection != null &&
                _channel != null)
            {
                return;
            }

            await _lock.WaitAsync();

            try
            {
                if (_connection != null &&
                    _channel != null)
                {
                    return;
                }

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
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task PublicarAsync<T>(
            string routingKey,
            T mensaje)
        {
            await InicializarAsync();

            var json =
                JsonSerializer.Serialize(mensaje);

            var body =
                Encoding.UTF8.GetBytes(json);

            var propiedades =
                new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent
                };

            var exchange =
                _configuration["RabbitMQ:Exchange"]
                ?? "gestiontaller.events";

            await _channel!.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: propiedades,
                body: body);
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
                await _channel.DisposeAsync();

            if (_connection != null)
                await _connection.DisposeAsync();

            _lock.Dispose();
        }
    }
}