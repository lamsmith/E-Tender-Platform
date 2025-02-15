namespace SharedLibrary.Models.Messages.UserEvents
{
    public class UserVerificationStatusMessage
    {
        public Guid UserId { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Reason { get; set; }
    }
}