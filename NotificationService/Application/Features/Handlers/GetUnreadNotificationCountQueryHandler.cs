using MediatR;
using NotificationService.Application.Features.Queries;
using NotificationService.Application.Common.Interface.Repositories;

namespace NotificationService.Application.Features.Handlers
{
    public class GetUnreadNotificationCountQueryHandler : IRequestHandler<GetUnreadNotificationCountQuery, int>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetUnreadNotificationCountQueryHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<int> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
        {
            return await _notificationRepository.GetUnreadCountAsync(request.UserId);
        }
    }
}