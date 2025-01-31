using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SharedLibrary.Models.Messages;

namespace NotificationService.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly int _maxRetries = 3;

        public EmailService(
            IConfiguration configuration,
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendStaffWelcomeEmailAsync(StaffWelcomeEmailMessage message)
        {
            var subject = "Welcome to the Company - Your Account Details";
            var body = GenerateWelcomeEmailBody(message);
            await SendEmailAsync(message.Email, subject, body);
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

        private string GenerateWelcomeEmailBody(StaffWelcomeEmailMessage message)
        {
            return $@"
                <h2>Welcome {message.FirstName} {message.LastName}!</h2>
                <p>Your account has been created successfully.</p>
                
                <h3>Your Account Details:</h3>
                <p>Temporary Password: <strong>{message.TempPassword}</strong></p>
                
                <p>Please log in and change your password at your earliest convenience.</p>
                
                <p>If you have any questions, please contact the IT support team.</p>
                
                <p>Best regards,<br>The IT Team</p>";
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
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount))); // Exponential backoff
                }
            }
        }
    }
}

