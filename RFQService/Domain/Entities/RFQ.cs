using RFQService.Domain.Common;
using RFQService.Domain.Enums;

namespace RFQService.Domain.Entities
{
    public class RFQ : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public VisibilityType Visibility { get; set; }
        public Guid CreatedByUserId { get; set; }
        public ICollection<RFQDocument> Documents { get; set; }
       

    }
}
