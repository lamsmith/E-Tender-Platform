using RFQService.Domain.Common;

namespace RFQService.Domain.Entities
{
    public class RFQDocument :BaseEntity
    {
        public Guid RFQId { get; set; }
        public RFQ RFQ { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileUrl { get; set; } 
        public DateTime UploadedAt { get; set; }
    }
}
