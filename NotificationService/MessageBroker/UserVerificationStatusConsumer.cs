using SharedLibrary.MessageBroker.Implementation;
using SharedLibrary.Models.Messages;
using NotificationService.Services;
using Microsoft.Extensions.Logging;

namespace NotificationService.MessageBroker
{
    public class UserVerificationStatusConsumer : MessageConsumer
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<UserVerificationStatusConsumer> _logger;

        public UserVerificationStatusConsumer(
            IEmailService emailService,
            ILogger<UserVerificationStatusConsumer> logger) : base(logger)
        {
            _emailService = emailService;
        }

        public override async Task ConsumeAsync(string messageType, string message)
        {
            try
            {
                var verificationMessage = System.Text.Json.JsonSerializer.Deserialize<UserVerificationStatusChangedMessage>(message);

                string subject = verificationMessage.Status == "Approved"
                    ? "Your Account Has Been Approved"
                    : "Account Verification Status Update";

                string template = await GetEmailTemplate(verificationMessage.Status);
                string body = template
                    .Replace("{{Status}}", verificationMessage.Status)
                    .Replace("{{Reason}}", verificationMessage.Reason ?? "");

                await _emailService.SendEmailAsync(verificationMessage.Email, subject, body);

                _logger.LogInformation(
                    "Sent verification {Status} email to user {UserId}",
                    verificationMessage.Status,
                    verificationMessage.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending verification status email");
                throw;
            }
        }

        private async Task<string> GetEmailTemplate(string status)
        {
            string templatePath = status == "Approved"
                ? "Templates/Email/VerificationApproved.html"
                : "Templates/Email/VerificationRejected.html";

            return await File.ReadAllTextAsync(templatePath);
        }
    }
}