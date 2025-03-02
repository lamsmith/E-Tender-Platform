using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Common.Interface.Services;
using NotificationService.Models;
using SharedLibrary.Models.Messages;

namespace NotificationService.Infrastructure.MessageConsumers
{
    public class StaffWelcomeEmailConsumer : IConsumer<StaffWelcomeEmailMessage>
    {
        private readonly IEmailService _emailService;
        private readonly ITemplateService _templateService;
        private readonly ILogger<StaffWelcomeEmailConsumer> _logger;

        public StaffWelcomeEmailConsumer(
            IEmailService emailService,
            ITemplateService templateService,
            ILogger<StaffWelcomeEmailConsumer> logger)
        {
            _emailService = emailService;
            _templateService = templateService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StaffWelcomeEmailMessage> context)
        {
            try
            {
                var message = context.Message;
                _logger.LogInformation("Processing welcome email for staff: {Email}", message.Email);

                var template = await _templateService.GetTemplateAsync("StaffWelcome");

                var variables = new Dictionary<string, string>
                {
                    { "FirstName", message.FirstName },
                    { "LastName", message.LastName },
                    { "TempPassword", message.TempPassword },
                    { "CurrentYear", DateTime.Now.Year.ToString() }
                };

                var emailBody = _templateService.ReplaceTemplateVariables(template, variables);
                if (string.IsNullOrEmpty(emailBody))
                {
                    _logger.LogWarning("Email template is empty, using fallback template");
                    emailBody = $"Welcome {message.FirstName} {message.LastName}!\n\nYour temporary password is: {message.TempPassword}\n\nPlease change your password upon first login.";
                }

                var emailModel = new EmailModel
                {
                    To = message.Email,
                    Subject = "Welcome to the Team - Your Account Details",
                    Body = emailBody,
                    IsHtml = true
                };

                await _emailService.SendEmailAsync(emailModel);

                _logger.LogInformation(
                    "Welcome email sent successfully to {Email}",
                    message.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending welcome email to {Email}",
                    context.Message.Email);
                throw;
            }
        }
    }
}