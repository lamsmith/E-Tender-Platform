using SharedLibrary.Models.Messages;
using NotificationService.Services;
using System.Text.Json;
using SharedLibrary.MessageBroker.Interfaces;

namespace NotificationService.MessageBroker
{
    public class OnboardingReminderMessageConsumer : IMessageConsumer
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<OnboardingReminderMessageConsumer> _logger;

        public OnboardingReminderMessageConsumer(
            IEmailService emailService,
            ILogger<OnboardingReminderMessageConsumer> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ConsumeAsync(string messageType, string message)
        {
            try
            {
                var reminderMessage = JsonSerializer.Deserialize<OnboardingReminderMessage>(message);

                string subject = "Complete Your Profile Setup";
                string body = await GetEmailTemplate(reminderMessage);

                await _emailService.SendEmailAsync(reminderMessage.Email, subject, body);

                _logger.LogInformation(
                    "Sent onboarding reminder to {Email} with {Count} incomplete tasks",
                    reminderMessage.Email,
                    reminderMessage.IncompleteTasks.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing onboarding reminder message");
                throw;
            }
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
    }
}