namespace SharedLibrary.Models.Messages
{
    public class UserProfileCompletedMessage
    {
        public Guid UserId { get; set; }
        public DateTime CompletedAt { get; set; }
        public bool IsVerified { get; set; }
    }
}