using MediatR;
using NotificationService.Application.Common.Interface.Repositories;
using NotificationService.Application.Features.Commands;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Features.Handlers
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Notification>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<CreateNotificationCommandHandler> _logger;

        public CreateNotificationCommandHandler(
            INotificationRepository notificationRepository,
            ILogger<CreateNotificationCommandHandler> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task<Notification> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _notificationRepository.AddAsync(request.Notification);
                return request.Notification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification for user: {UserId}", request.Notification.UserId);
                throw;
            }
        }
    }
}