using NotificationService.Services;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using SharedLibrary.MessageBroker.Interfaces;
using System.Text;
using SharedLibrary.Models.Messages;
using System.Text.Json;

public class UserVerificationMessageConsumer : IMessageConsumer
{
    private readonly IEmailService _emailService;
    private readonly ILogger<UserVerificationMessageConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private string _consumerTag;

    public UserVerificationMessageConsumer(IRabbitMQConnection connection, IEmailService emailService, ILogger<UserVerificationMessageConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
        _connection = connection.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public async Task ConsumeAsync(string messageType, string message)
    {
        try
        {
            var verificationMessage = JsonSerializer.Deserialize<UserVerificationMessage>(message);

            string subject = verificationMessage.Status == "Approved"
                ? "Your Account Has Been Approved"
                : "Account Verification Status Update";

            string body = await GetEmailTemplate(verificationMessage);

            await _emailService.SendEmailAsync(verificationMessage.Email, subject, body);

            _logger.LogInformation(
                "Sent verification {Status} email to {Email}",
                verificationMessage.Status,
                verificationMessage.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing verification message");
            throw;
        }
    }



    private async Task<string> GetEmailTemplate(UserVerificationMessage message)
    {
        string templatePath = message.Status == "Approved"
            ? "Templates/Email/VerificationApproved.html"
            : "Templates/Email/VerificationRejected.html";

        string template = await File.ReadAllTextAsync(templatePath);

        return template
            .Replace("{{Email}}", message.Email)
            .Replace("{{Status}}", message.Status)
            .Replace("{{Notes}}", message.Notes ?? "")
            .Replace("{{Date}}", message.VerifiedAt.ToString("dd MMM yyyy"));
    }
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
}
