namespace SharedLibrary.Models.Messages
{
    public class BidStatusUpdatedMessage
    {
        public string Type { get; set; }
        public Guid BidId { get; set; }
        public string NewStatus { get; set; }
        public string Notes { get; set; }
        public DateTime Timestamp { get; set; }
    }
}