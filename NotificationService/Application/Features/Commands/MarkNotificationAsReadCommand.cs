using MediatR;

namespace NotificationService.Application.Features.Commands
{
    public class MarkNotificationAsReadCommand : IRequest<bool>
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
    }
}