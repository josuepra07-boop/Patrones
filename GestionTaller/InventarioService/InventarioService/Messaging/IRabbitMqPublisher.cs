namespace InventarioService.Messaging
{
    public interface IRabbitMqPublisher
    {
        Task PublicarAsync<T>(
            string routingKey,
            T mensaje);
    }
}