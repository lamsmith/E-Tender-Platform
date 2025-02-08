using SharedLibrary.Enums;

namespace SharedLibrary.Models.Messages
{
    public class UserStatusChangedMessage
    {
        public Guid UserId { get; set; }
        public AccountStatus NewStatus { get; set; }
        public string? Reason { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}