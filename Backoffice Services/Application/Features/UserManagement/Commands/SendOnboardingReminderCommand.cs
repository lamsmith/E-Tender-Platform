using MediatR;

namespace Backoffice_Services.Application.Features.UserManagement.Commands
{
    public class SendOnboardingReminderCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; set; }
        public int ReminderCount { get; set; }
    }
}



