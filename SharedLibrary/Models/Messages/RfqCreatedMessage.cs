namespace SharedLibrary.Models.Messages
{
    public class RfqCreatedMessage
    {
        public string RfqId { get; set; }
        public string Title { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Deadline { get; set; }
    }
}