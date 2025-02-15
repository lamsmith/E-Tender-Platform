namespace SharedLibrary.Models.Messages.RfqEvents
{
    public class RfqCreatedMessage
    {
        public Guid RfqId { get; set; }
        public string ContractTitle { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string Visibility { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Deadline { get; set; }
    }
}