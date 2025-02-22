namespace SharedLibrary.Models.Messages.RfqEvents
{
    public class RfqUpdatedMessage
    {
        public Guid RfqId { get; set; }
        public string ContractTitle { get; set; }
        public string Visibility { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime Deadline { get; set; }
        public List<string> RecipientEmails { get; set; } = new();
    }
}