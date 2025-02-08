using Backoffice_Services.Application.DTO.BidManagement.Common;

namespace Backoffice_Services.Application.DTO.BidManagement.Responses
{
    public class BidResponseModel
    {
        public Guid Id { get; set; }
        public Guid RfqId { get; set; }
        public string RfqTitle { get; set; }
        public Guid UserId { get; set; }
        public string Proposal { get; set; }
        public decimal Amount { get; set; }
        public BidStatus Status { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public List<BidDocumentResponse> Documents { get; set; }
    }

    public class BidDocumentResponse
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string FileUrl { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}