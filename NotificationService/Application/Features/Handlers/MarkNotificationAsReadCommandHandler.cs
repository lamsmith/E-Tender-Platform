using MediatR;
using NotificationService.Application.Common.Interface.Repositories;
using NotificationService.Application.Features.Commands;

namespace NotificationService.Application.Features.Handlers
{
    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, bool>
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<bool> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _notificationRepository.GetByIdAsync(request.NotificationId);
            if (notification == null || notification.UserId != request.UserId)
                return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _notificationRepository.UpdateAsync(notification);
            return true;
        }
    }
}