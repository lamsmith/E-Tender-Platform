namespace SharedLibrary.Models.Messages
{
    public class NotificationMessage
    {
        public Guid UserId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}