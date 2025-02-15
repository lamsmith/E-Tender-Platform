namespace SharedLibrary.Models.Messages.BidEvents
{
    public class BidSubmittedMessage
    {
        public Guid BidId { get; set; }
        public Guid UserId { get; set; }
        public Guid RfqId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}