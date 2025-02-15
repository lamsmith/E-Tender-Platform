namespace SharedLibrary.Models.Messages.EmailEvents
{
    public class EmailNotificationMessage
    {
        public Guid UserId { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public DateTime SentAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}