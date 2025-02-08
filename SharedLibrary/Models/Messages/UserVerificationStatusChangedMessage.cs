namespace SharedLibrary.Models.Messages
{
    public class UserVerificationStatusChangedMessage
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }  // "Approved" or "Rejected"
        public string? Reason { get; set; }
        public DateTime VerifiedAt { get; set; }
    }
}