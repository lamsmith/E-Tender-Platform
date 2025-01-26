using MediatR;
using NotificationService.Application.Common.Interface.Repositories;
using NotificationService.Application.Features.Queries;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Paging;

namespace NotificationService.Application.Features.Handlers
{
    public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, PaginatedList<Notification>>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetUserNotificationsQueryHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<PaginatedList<Notification>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            return await _notificationRepository.GetUserNotificationsAsync(request.UserId, request.PageRequest);
        }
    }
}