using MediatR;

namespace NotificationService.Application.Features.Queries
{
    public class GetUnreadNotificationCountQuery : IRequest<int>
    {
        public Guid UserId { get; set; }
    }
}