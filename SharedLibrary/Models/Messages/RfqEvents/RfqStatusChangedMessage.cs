namespace SharedLibrary.Models.Messages.RfqEvents
{
    public class RfqStatusChangedMessage
    {
        public Guid RfqId { get; set; }
        public string NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}