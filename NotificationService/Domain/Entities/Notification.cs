using NotificationService.Domain.Common;
using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; } 
        public DateTime Timestamp { get; set; } 
        public bool IsRead { get; set; }
    }
}
