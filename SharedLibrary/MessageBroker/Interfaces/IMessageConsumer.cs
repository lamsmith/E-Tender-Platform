namespace SharedLibrary.MessageBroker.Interfaces
{
    public interface IMessageConsumer
    {
        Task ConsumeAsync<T>(T message) where T : class;
    }
}