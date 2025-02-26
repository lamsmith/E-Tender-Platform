using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Common.Interface.Services;
using NotificationService.Models;
using NotificationService.Services;
using SharedLibrary.Models.Messages.RfqEvents;

namespace NotificationService.Infrastructure.MessageConsumers
{
    public class RfqEmailNotificationConsumer : IConsumer<RfqEmailNotificationMessage>
    {
        private readonly IEmailService _emailService;
        private readonly ITemplateService _templateService;
        private readonly ILogger<RfqEmailNotificationConsumer> _logger;

        public RfqEmailNotificationConsumer(
            IEmailService emailService,
            ITemplateService templateService,
            ILogger<RfqEmailNotificationConsumer> logger)
        {
            _emailService = emailService;
            _templateService = templateService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RfqEmailNotificationMessage> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Processing RFQ email notifications for RFQ {RfqId}, Recipients: {RecipientCount}",
                message.RfqId,
                message.RecipientEmails.Count);

            var template = await _templateService.GetTemplateAsync("RfqNotification");

            foreach (var email in message.RecipientEmails)
            {
                try
                {
                    var variables = new Dictionary<string, string>
                    {
                        { "ContractTitle", message.ContractTitle },
                        { "RfqLink", message.RfqLink },
                        { "Deadline", message.Deadline.ToString("dd MMM yyyy HH:mm") },
                        { "CurrentYear", DateTime.Now.Year.ToString() }
                    };

                    var emailBody = _templateService.ReplaceTemplateVariables(template, variables);
                    if (string.IsNullOrEmpty(emailBody))
                    {
                        _logger.LogWarning("Email body is null or empty for {Email}, using fallback.", email);
                        emailBody = $"A new RFQ '{message.ContractTitle ?? "Untitled"}' is available at {message.RfqLink ?? "no link"}. Deadline: {message.Deadline}";
                    }

                    var emailModel = new EmailModel
                    {
                        To = email,
                        Subject = $"New RFQ Invitation: {message.ContractTitle}",
                        Body = emailBody,
                        IsHtml = true
                    };

                    await _emailService.SendEmailAsync(emailModel);

                    _logger.LogInformation(
                        "RFQ notification email sent successfully to {Email} for RFQ {RfqId}",
                        email,
                        message.RfqId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error sending RFQ notification email to {Email} for RFQ {RfqId}",
                        email,
                        message.RfqId);
                }
            }
        }
    }
}