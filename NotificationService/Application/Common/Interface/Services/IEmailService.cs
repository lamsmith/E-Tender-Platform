using NotificationService.Models;
using SharedLibrary.Models.Messages;

namespace NotificationService.Application.Common.Interface.Services
{
    public interface IEmailService
    {
        Task SendStaffWelcomeEmailAsync(StaffWelcomeEmailMessage message);
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailAsync(EmailModel emailModel);
    }
}