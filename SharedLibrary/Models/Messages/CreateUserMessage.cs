namespace SharedLibrary.Models.Messages
{
    public class CreateUserMessage
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}