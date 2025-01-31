namespace SharedLibrary.MessageBroker
{
    public interface IMessageConsumer
    {
        Task ConsumeAsync(string messageType, string message);
    }
}