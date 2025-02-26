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
            _configuration = configuration;
            _templateService = templateService;
            _logger = logger;
        }

        public async Task SendStaffWelcomeEmailAsync(StaffWelcomeEmailMessage message)
        {
            try
            {
                var template = await _templateService.GetTemplateAsync("StaffWelcome");
                var variables = new Dictionary<string, string>
                {
                    { "FirstName", message.FirstName },
                    { "LastName", message.LastName },
                    { "TempPassword", message.TempPassword }
                };

                var body = _templateService.ReplaceTemplateVariables(template, variables);
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
            await SendEmailAsync(emailModel.To, emailModel.Subject, emailModel.Body);
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["Email:FromAddress"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

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

