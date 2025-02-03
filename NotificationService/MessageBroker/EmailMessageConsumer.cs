using SharedLibrary.MessageBroker;
using SharedLibrary.Models.Messages;
using NotificationService.Services;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SharedLibrary.MessageBroker.Interfaces;

namespace NotificationService.MessageBroker
{
    public class EmailMessageConsumer : IMessageConsumer
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailMessageConsumer> _logger;

        public EmailMessageConsumer(
            IEmailService emailService,
            ILogger<EmailMessageConsumer> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ConsumeAsync(string messageType, string message)
        {
            try
            {
                switch (messageType)
                {
                    case MessageTypes.StaffWelcomeEmail:
                        var welcomeMessage = JsonSerializer.Deserialize<StaffWelcomeEmailMessage>(message);
                        await _emailService.SendStaffWelcomeEmailAsync(welcomeMessage);
                        break;

                    default:
                        _logger.LogWarning("Unknown message type received: {MessageType}", messageType);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing email message of type: {MessageType}", messageType);
                throw;
            }
        }
    }
}
