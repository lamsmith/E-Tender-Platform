using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace YourNamespace.Infrastructure.MessageBroker
{
    public class RabbitMQConsumer<T>
    {
        private readonly IModel _channel;
        private readonly string _queueName;

        public RabbitMQConsumer(RabbitMQConnection connection, string queueName)
        {
            _channel = connection.Channel;
            _queueName = queueName;

            _channel.QueueDeclare(queue: _queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
        }

        public void Consume(Action<T> messageHandler)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var deserializedMessage = JsonSerializer.Deserialize<T>(message);

                messageHandler(deserializedMessage);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: _queueName,
                                autoAck: false,
                                consumer: consumer);
        }
    }
}