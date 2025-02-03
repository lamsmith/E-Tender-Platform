namespace SharedLibrary.Models.Messages
{
    public class UserVerificationMessage
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }  // "Approved" or "Rejected"
        public string? Notes { get; set; }
        public DateTime VerifiedAt { get; set; }
    }
}