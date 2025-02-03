namespace SharedLibrary.MessageBroker.Interfaces
{
    public interface IMessageConsumer
    {
        Task ConsumeAsync(string messageType, string message);
        Task StartConsumingAsync(string queueName, CancellationToken cancellationToken);
        Task StopConsumingAsync();
    }
}