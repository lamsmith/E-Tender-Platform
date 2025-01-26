using RFQService.Domain.Enums;

namespace RFQService.Application.DTO.Requests
{
    public class RFQUpdateRequestModel
    {
        public string ContractTitle { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime? SubmissionDeadline { get; set; }
        public List<RFQDocumentRequest> Documents { get; set; }
    }

    public class RFQDocumentRequest
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string FileUrl { get; set; }
    }
}
