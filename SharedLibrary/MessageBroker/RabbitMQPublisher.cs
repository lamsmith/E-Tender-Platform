using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace SharedLibrary.MessageBroker
{
    public class RabbitMQPublisher : IMessagePublisher
    {
        private readonly IModel _channel;

        public RabbitMQPublisher(RabbitMQConnection connection)
        {
            _channel = connection.Channel;
        }

        public void PublishMessage<T>(string queueName, T message)
        {
            _channel.QueueDeclare(queue: queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            _channel.BasicPublish(exchange: "",
                                routingKey: queueName,
                                basicProperties: null,
                                body: body);
        }
    }
}