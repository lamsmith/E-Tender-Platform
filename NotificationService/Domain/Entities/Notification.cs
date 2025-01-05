using NotificationService.Domain.Common;

namespace NotificationService.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; } 
        public string Message { get; set; } 
        public DateTime SentAt { get; set; } 
        public bool IsRead { get; set; }
    }
}
