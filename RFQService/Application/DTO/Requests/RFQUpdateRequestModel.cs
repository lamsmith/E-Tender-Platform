using RFQService.Domain.Enums;

namespace RFQService.Application.DTO.Requests
{
    public class RFQUpdateRequestModel
    {
        public string ContractTitle { get; set; }
        public string ScopeOfSupply { get; set; }
        public string PaymentTerms { get; set; }
        public string DeliveryTerms { get; set; }
        public string OtherInformation { get; set; }
        public DateTime? Deadline { get; set; }
        public VisibilityType? Visibility { get; set; }
    }
}
