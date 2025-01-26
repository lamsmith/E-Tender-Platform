using MediatR;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Paging;

namespace NotificationService.Application.Features.Queries
{
    public class GetUserNotificationsQuery : IRequest<PaginatedList<Notification>>
    {
        public Guid UserId { get; set; }
        public PageRequest PageRequest { get; set; }
    }
}