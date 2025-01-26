using MediatR;

namespace NotificationService.Application.Features.Commands
{
    public class MarkAllNotificationsAsReadCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
    }
}