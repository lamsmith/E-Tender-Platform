using MediatR;
using NotificationService.Application.Features.Commands;
using NotificationService.Application.Common.Interface.Repositories;

namespace NotificationService.Application.Features.Handlers
{
    public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, bool>
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkAllNotificationsAsReadCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<bool> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            await _notificationRepository.MarkAllAsReadAsync(request.UserId);
            return true;
        }
    }
}