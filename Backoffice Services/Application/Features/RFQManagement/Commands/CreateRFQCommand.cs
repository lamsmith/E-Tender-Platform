using MediatR;
using Backoffice_Services.Application.DTO.RFQManagement.Responses;
using Backoffice_Services.Application.DTO.RFQManagement.Common;

namespace Backoffice_Services.Application.Features.RFQManagement.Commands
{
    public class CreateRFQCommand : IRequest<RFQResponseModel>
    {
        public string ContractTitle { get; set; }
        public string ScopeOfSupply { get; set; }
        public string PaymentTerms { get; set; }
        public string DeliveryTerms { get; set; }
        public string OtherInformation { get; set; }
        public DateTime Deadline { get; set; }
        public VisibilityType Visibility { get; set; }
        public Guid CreatedByUserId { get; set; }
        public List<RFQDocumentModel> Documents { get; set; } = new();
    }

    public class RFQDocumentModel
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileUrl { get; set; }
    }
}