using RabbitMQ.Client;

namespace SharedLibrary.MessageBroker.Interfaces
{
    public interface IRabbitMQConnection
    {
        IConnection CreateConnection();
        bool IsConnected { get; }
        bool TryConnect();
    }
}
