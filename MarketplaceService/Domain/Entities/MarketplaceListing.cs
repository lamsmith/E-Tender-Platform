using MarketplaceService.Domain.Common;

namespace MarketplaceService.Domain.Entities
{
    public class MarketplaceListing : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid RFQId { get; set; } 
        public Guid UserId { get; set; } 
        public bool IsPublic { get; set; } 
        public DateTime ListedAt { get; set; }
    }
}
