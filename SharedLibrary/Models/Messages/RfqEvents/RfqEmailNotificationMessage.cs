namespace SharedLibrary.Models.Messages.RfqEvents
{
    public class RfqEmailNotificationMessage
    {
        public Guid RfqId { get; set; }
        public string ContractTitle { get; set; }
        public List<string> RecipientEmails { get; set; } = new();
        public string RfqLink { get; set; }
        public DateTime Deadline { get; set; }
    }
}