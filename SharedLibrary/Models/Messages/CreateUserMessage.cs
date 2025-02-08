namespace SharedLibrary.Models.Messages
{
    public class CreateUserMessage
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }  // "MSME" or "Corporate"
        public DateTime CreatedAt { get; set; }
    }
}