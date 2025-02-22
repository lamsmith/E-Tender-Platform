using MediatR;
using RFQService.Domain.Enums;

namespace RFQService.Application.Features.Commands
{
    public class UpdateRFQCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string ContractTitle { get; set; }
        public string CompanyName { get; set; }
        public string ScopeOfSupply { get; set; }
        public string PaymentTerms { get; set; }
        public string DeliveryTerms { get; set; }
        public string OtherInformation { get; set; }
        public Status Status { get; set; }
        public DateTime Deadline { get; set; }
        public VisibilityType Visibility { get; set; }
        public List<string> RecipientEmails { get; set; } = new();
    }
}
