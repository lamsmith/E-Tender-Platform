namespace BidService.Application.DTO.Requests
{
    public class BidCreationRequestModel
    {
        public Guid RFQId { get; set; }
        public Guid UserId { get; set; }
        public decimal BidAmount { get; set; }
        public List<DocumentUploadRequest> Documents { get; set; } = new List<DocumentUploadRequest>();
    }

    public class DocumentUploadRequest
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string FileUrl { get; set; }
    }
}
