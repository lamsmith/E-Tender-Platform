using BidService.Domain.Common;

namespace BidService.Domain.Entities
{
    public class File : BaseEntity
    {
        public Guid BidId { get; set; }
        public Bid Bid { get; set; }
        public string FileName { get; set; } 
        public string FileUrl { get; set; } 
        public string FileType { get; set; }
      
    }
}
