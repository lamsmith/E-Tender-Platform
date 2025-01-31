using SharedLibrary.Models.Messages;

namespace NotificationService.Services
{
    public interface IEmailService
    {
        Task SendStaffWelcomeEmailAsync(StaffWelcomeEmailMessage message);
        Task SendEmailAsync(string to, string subject, string body);
    }
}