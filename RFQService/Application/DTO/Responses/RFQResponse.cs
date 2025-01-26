namespace RFQService.Application.DTO.Responses
{
    public class RFQResponse
    {
        public Guid Id { get; set; }
        public Guid CreatedBy { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime SubmissionDeadline { get; set; }
        public int TotalBids { get; set; }
        public List<string> RequiredDocuments { get; set; }
        public List<string> RequiredCertifications { get; set; }
    }
}