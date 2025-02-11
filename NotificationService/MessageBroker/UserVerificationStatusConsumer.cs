using SharedLibrary.MessageBroker.Interfaces;
using SharedLibrary.Models.Messages;
using NotificationService.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace NotificationService.MessageBroker
{
    public class UserVerificationStatusConsumer : IMessageConsumer
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<UserVerificationStatusConsumer> _logger;
        private readonly IConnection _connection;
        private IModel _channel;
        private string _consumerTag;

        public UserVerificationStatusConsumer(
            IRabbitMQConnection rabbitMQConnection,
            IEmailService emailService,
            ILogger<UserVerificationStatusConsumer> logger)
        {
            _connection = rabbitMQConnection.CreateConnection();
            _channel = _connection.CreateModel();
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ConsumeAsync(string messageType, string message)
        {
            try
            {
                var verificationMessage = JsonSerializer.Deserialize<UserVerificationStatusChangedMessage>(message);
                var emailTemplate = await GetEmailTemplate(verificationMessage.Status);

                await _emailService.SendEmailAsync(
                    verificationMessage.Email,
                    $"Account Verification {verificationMessage.Status}",
                    emailTemplate.Replace("{{Notes}}", verificationMessage.Reason ?? ""));

                _logger.LogInformation(
                    "Sent verification {Status} email to {Email}",
                    verificationMessage.Status,
                    verificationMessage.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing verification status message");
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

        private async Task<string> GetEmailTemplate(string status)
        {
            string templatePath = status == "Approved"
                ? "Templates/Email/VerificationApproved.html"
                : "Templates/Email/VerificationRejected.html";

            return await File.ReadAllTextAsync(templatePath);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}