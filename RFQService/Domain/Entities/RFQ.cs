using RFQService.Domain.Common;
using RFQService.Domain.Enums;

namespace RFQService.Domain.Entities
{
    public class RFQ : BaseEntity
    {
        public string  ContractTitle { get; set; }
        public string ScopeOfSupply { get; set; }
        public string PaymentTerms { get; set; }
        public string DeliveryTerms { get; set; }
        public string OtherInformation { get; set; }
        public DateTime Deadline { get; set; }
        public Status Status { get; set; }
        public VisibilityType Visibility { get; set; }
        public Guid CreatedByUserId { get; set; }
        public ICollection<RFQRecipient> Recipients { get; set; } = new List<RFQRecipient>();
        public ICollection<RFQDocument> Documents { get; set; }
       

    }
}
