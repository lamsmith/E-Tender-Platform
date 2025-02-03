using MediatR;

namespace Backoffice_Services.Application.Features.UserManagement.Commands
{
    public class SendOnboardingReminderCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public List<string> IncompleteTasks { get; set; } = new();
        public string? Notes { get; set; }
    }
}



