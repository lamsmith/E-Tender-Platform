using MediatR;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Features.Commands
{
    public class CreateNotificationCommand : IRequest<Notification>
    {
        public Notification Notification { get; set; }
    }
}