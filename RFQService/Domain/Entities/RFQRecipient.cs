namespace RFQService.Domain.Entities
{
    public class RFQRecipient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RFQId { get; set; }
        public  RFQ RFQ { get; set; }
        public Guid? UserId { get; set; }
        public string Email { get; set; } = null!;
    }
}
