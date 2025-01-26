namespace RFQService.Application.DTO.Requests
{
    public class UpdateRFQRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime? SubmissionDeadline { get; set; }
        public List<string> RequiredDocuments { get; set; }
        public List<string> RequiredCertifications { get; set; }
    }
}