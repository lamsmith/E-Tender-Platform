namespace SharedLibrary.Models.Messages.RfqEvents
{
    public class RfqStatusUpdatedMessage
    {
        public Guid RfqId { get; set; }
        public string NewStatus { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Reason { get; set; }
    }
}