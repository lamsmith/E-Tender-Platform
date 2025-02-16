namespace SharedLibrary.Models.Messages.BidEvents
{
    public class BidSubmittedMessage
    {
        public Guid BidId { get; set; }
        public Guid RfqId { get; set; }
        public Guid UserId { get; set; }
        public string Proposal { get; set; }
        public decimal CostOfProduct { get; set; }
        public decimal CostOfShipping { get; set; }
        public decimal Discount { get; set; }
        public DateTime SubmittedAt { get; set; }
        public BidFile? CompanyProfile { get; set; }
        public BidFile? ProjectPlan { get; set; }
        public BidFile? ProposalFile { get; set; }
    }

    public class BidFile
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string FileUrl { get; set; }
    }
}