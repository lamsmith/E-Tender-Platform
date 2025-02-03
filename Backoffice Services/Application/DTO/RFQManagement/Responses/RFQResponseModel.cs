using Backoffice_Services.Application.DTO.RFQManagement.Common;

namespace Backoffice_Services.Application.DTO.RFQManagement.Responses
{
    public class RFQResponseModel
    {
        public Guid Id { get; set; }
        public string ContractTitle { get; set; }
        public string ScopeOfSupply { get; set; }
        public string PaymentTerms { get; set; }
        public string DeliveryTerms { get; set; }
        public string OtherInformation { get; set; }
        public DateTime Deadline { get; set; }
        public VisibilityType Visibility { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<RFQDocumentResponseModel> Documents { get; set; }
    }

    public class RFQDocumentResponseModel
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileUrl { get; set; }
    }

  
}

