namespace SharedLibrary.Models.Messages.RfqEvents
{
    public class RfqCreatedMessage
    {
        public Guid RfqId { get; set; }
        public string ContractTitle { get; set; }
        public string ScopeOfSupply { get; set; }
        public string PaymentTerms { get; set; }
        public string DeliveryTerms { get; set; }
        public string OtherInformation { get; set; }
        public string CompanyName { get; set; }
        public DateTime Deadline { get; set; }
        public string Visibility { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedByUserId { get; set; }
    }
}