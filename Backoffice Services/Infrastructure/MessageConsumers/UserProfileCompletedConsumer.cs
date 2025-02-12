using SharedLibrary.MessageBroker.Interfaces;
using SharedLibrary.Models.Messages;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Backoffice_Services.Infrastructure.ExternalServices;

namespace Backoffice_Services.Infrastructure.MessageConsumers
{
    public class UserProfileCompletedConsumer : IMessageConsumer
    {
        private readonly IUserProfileServiceClient _userProfileServiceClient;
        private readonly ILogger<UserProfileCompletedConsumer> _logger;
        private readonly IConnection _connection;
        private IModel _channel;
        private string _consumerTag;

        public UserProfileCompletedConsumer(
            IRabbitMQConnection rabbitMQConnection,
            IUserProfileServiceClient userProfileServiceClient,
            ILogger<UserProfileCompletedConsumer> logger)
        {
            _connection = rabbitMQConnection.CreateConnection();
            _channel = _connection.CreateModel();
            _userProfileServiceClient = userProfileServiceClient;
            _logger = logger;
        }

        public async Task ConsumeAsync(string messageType, string message)
        {
            try
            {
                var profileCompletedMessage = JsonSerializer.Deserialize<UserProfileCompletedMessage>(message);
                var userDetails = await _userProfileServiceClient.GetUserDetailsAsync(profileCompletedMessage.UserId);

                _logger.LogInformation("Processed profile completion for user: {Email}", userDetails.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing user profile completion message");
                throw;
            }
        }

        public Task StartConsumingAsync(string queueName, CancellationToken cancellationToken)
        {
            _channel.QueueDeclare(queue: queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var messageType = ea.BasicProperties.Type;

                try
                {
                    await ConsumeAsync(messageType, message);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _consumerTag = _channel.BasicConsume(queue: queueName,
                                               autoAck: false,
                                               consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopConsumingAsync()
        {
            if (!string.IsNullOrEmpty(_consumerTag))
            {
                _channel?.BasicCancel(_consumerTag);
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