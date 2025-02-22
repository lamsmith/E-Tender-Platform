using Backoffice_Services.Application.DTO.RFQManagement.Common;

namespace Backoffice_Services.Application.DTO.RFQManagement.Requests
{
    public class RFQCreationRequestModel
    {
        public string ContractTitle { get; set; }
        public string ScopeOfSupply { get; set; }
        public string PaymentTerms { get; set; }
        public string DeliveryTerms { get; set; }
        public string OtherInformation { get; set; }
        public DateTime Deadline { get; set; }
        public VisibilityType Visibility { get; set; }
        public Guid CreatedByUserId { get; set; }

        // New property for MSME recipient emails
        public List<string> RecipientEmails { get; set; } = new();
    }

}
