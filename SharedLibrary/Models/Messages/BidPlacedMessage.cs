namespace SharedLibrary.Models.Messages
{
    public class BidPlacedMessage
    {
        public string BidId { get; set; }
        public string UserId { get; set; }
        public string RfqId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PlacedAt { get; set; }
    }
}