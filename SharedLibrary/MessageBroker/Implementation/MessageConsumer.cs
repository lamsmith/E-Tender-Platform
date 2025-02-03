using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using SharedLibrary.MessageBroker.Interfaces;
using Microsoft.Extensions.Logging;

namespace SharedLibrary.MessageBroker.Implementation
{
    public abstract class MessageConsumer : IMessageConsumer, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger _logger;
        private string _consumerTag;

        protected MessageConsumer(IRabbitMQConnection connection, ILogger logger)
        {
            _connection = connection.CreateConnection();
            _channel = _connection.CreateModel();
            _logger = logger;
        }

        public abstract Task ConsumeAsync(string messageType, string message);

        public Task StartConsumingAsync(string queueName, CancellationToken cancellationToken)
        {
            _channel.QueueDeclare(queueName, true, false, false);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var messageType = ea.RoutingKey;

                    await ConsumeAsync(messageType, message);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from queue {QueueName}", queueName);
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _consumerTag = _channel.BasicConsume(queueName, false, consumer);
            return Task.CompletedTask;
        }

        public Task StopConsumingAsync()
        {
            if (!string.IsNullOrEmpty(_consumerTag))
            {
                _channel.BasicCancel(_consumerTag);
                _consumerTag = null;
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}