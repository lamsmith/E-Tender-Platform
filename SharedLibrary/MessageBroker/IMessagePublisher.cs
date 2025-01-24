namespace SharedLibrary.MessageBroker
{
    public interface IMessagePublisher
    {
        void PublishMessage<T>(string queueName, T message);
    }
}