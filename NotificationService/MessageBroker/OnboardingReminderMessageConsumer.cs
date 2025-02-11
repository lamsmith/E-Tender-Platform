using SharedLibrary.Models.Messages;
using NotificationService.Services;
using System.Text.Json;
using SharedLibrary.MessageBroker.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace NotificationService.MessageBroker
{
    public class OnboardingReminderMessageConsumer : IMessageConsumer
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<OnboardingReminderMessageConsumer> _logger;
        private readonly IConnection _connection;
        private IModel _channel;
        private string _consumerTag;

        public OnboardingReminderMessageConsumer(
            IRabbitMQConnection rabbitMQConnection,
            IEmailService emailService,
            ILogger<OnboardingReminderMessageConsumer> logger)
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
                var reminderMessage = JsonSerializer.Deserialize<OnboardingReminderMessage>(message);
                var emailTemplate = await GetEmailTemplate(reminderMessage);

                await _emailService.SendEmailAsync(
                    reminderMessage.Email,
                    "Complete Your Profile Setup",
                    emailTemplate);

                _logger.LogInformation("Sent onboarding reminder to {Email}", reminderMessage.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing onboarding reminder message");
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

        private async Task<string> GetEmailTemplate(OnboardingReminderMessage message)
        {
            string template = await File.ReadAllTextAsync("Templates/Email/OnboardingReminder.html");

            var tasksList = string.Join("\n", message.IncompleteTasks.Select(task => $"<li>{task}</li>"));

            return template
                .Replace("{{Email}}", message.Email)
                .Replace("{{DaysRegistered}}", message.DaysRegistered.ToString())
                .Replace("{{IncompleteTasks}}", tasksList);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}