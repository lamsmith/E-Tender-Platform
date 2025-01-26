using MediatR;
using NotificationService.Application.Features.Commands;
using NotificationService.Application.Common.Interface.Repositories;

namespace NotificationService.Application.Features.Handlers
{
    public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, bool>
    {
        private readonly INotificationRepository _notificationRepository;

        public DeleteNotificationCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<bool> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = await _notificationRepository.GetByIdAsync(request.NotificationId);
            if (notification == null || notification.UserId != request.UserId)
                return false;

            await _notificationRepository.DeleteAsync(request.NotificationId);
            return true;
        }
    }
}