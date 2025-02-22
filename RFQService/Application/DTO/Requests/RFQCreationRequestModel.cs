using RFQService.Domain.Enums;

namespace RFQService.Application.DTO.Requests
{
    public class RFQCreationRequestModel
    {
        public string ContractTitle { get; set; }
        public string CompanyName { get; set; }
        public string ScopeOfSupply { get; set; }
        public string PaymentTerms { get; set; }
        public string DeliveryTerms { get; set; }
        public string OtherInformation { get; set; }
        public DateTime Deadline { get; set; }
        public VisibilityType Visibility { get; set; }
        public List<string> RecipientEmails { get; set; } = new();
        public Guid CreatedByUserId { get; set; }
    }
}
