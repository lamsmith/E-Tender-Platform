using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NotificationService.Application.Common.Interface.Services;
using NotificationService.Models;
using SharedLibrary.Models.Messages;
using System.Text.RegularExpressions;

namespace NotificationService.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly ITemplateService _templateService;
        private readonly int _maxRetries = 3;

        public EmailService(
            IConfiguration configuration,
            ITemplateService templateService,
            ILogger<EmailService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendStaffWelcomeEmailAsync(StaffWelcomeEmailMessage message)
        {
            try
            {
                if (message == null)
                    throw new ArgumentNullException(nameof(message), "StaffWelcomeEmailMessage cannot be null.");

                _logger.LogInformation("Fetching email template for StaffWelcome...");
                var template = await _templateService.GetTemplateAsync("StaffWelcome");

                if (string.IsNullOrWhiteSpace(template))
                {
                    _logger.LogError("Email template 'StaffWelcome' is empty or not found.");
                    throw new InvalidOperationException("Email template is missing or empty.");
                }

                var variables = new Dictionary<string, string>
                {
                    { "FirstName", message.FirstName ?? "[Unknown]" },
                    { "LastName", message.LastName ?? "[Unknown]" },
                    { "TempPassword", message.TempPassword ?? "[No Password]" }
                };

                _logger.LogInformation("Replacing template variables...");
                var body = _templateService.ReplaceTemplateVariables(template, variables);

                if (string.IsNullOrWhiteSpace(body))
                {
                    _logger.LogError("Generated email body is empty after replacing template variables.");
                    throw new InvalidOperationException("Email body is missing or empty.");
                }

                _logger.LogInformation("Sending email to {Email}...", message.Email);
                await SendEmailAsync(message.Email, "Welcome to the Company - Your Account Details", body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending welcome email to {Email}", message.Email);
                throw;
            }
        }

        public async Task SendEmailAsync(EmailModel emailModel)
        {
            if (emailModel == null)
                throw new ArgumentNullException(nameof(emailModel), "EmailModel cannot be null.");

            await SendEmailAsync(emailModel.To, emailModel.Subject, emailModel.Body);
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Recipient email cannot be empty.", nameof(to));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Email subject cannot be empty.", nameof(subject));

            if (string.IsNullOrWhiteSpace(body))
            {
                _logger.LogError("Email body is empty. Email will not be sent.");
                throw new ArgumentException("Email body cannot be null or empty.");
            }

            _logger.LogInformation("Preparing email to send...");
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["Email:SmtpHost"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            _logger.LogInformation("Email body: {Body}", body);
            await SendEmailWithRetryAsync(email);
        }

        private async Task SendEmailWithRetryAsync(MimeMessage email)
        {
            var retryCount = 0;
            while (retryCount < _maxRetries)
            {
                try
                {
                    using var smtp = new SmtpClient();
                    await smtp.ConnectAsync(
                        _configuration["Email:SmtpHost"],
                        int.Parse(_configuration["Email:SmtpPort"]),
                        SecureSocketOptions.StartTls);

                    await smtp.AuthenticateAsync(
                        _configuration["Email:Username"],
                        _configuration["Email:Password"]);

                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);

                    _logger.LogInformation("Email sent successfully to: {To}", email.To);
                    return;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount == _maxRetries)
                    {
                        _logger.LogError(ex, "Failed to send email after {RetryCount} attempts", _maxRetries);
                        throw;
                    }

                    _logger.LogWarning(ex, "Failed to send email (attempt {RetryCount}), retrying...", retryCount);
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount)));
                }
            }
        }
    }
}
