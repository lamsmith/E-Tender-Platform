using BidService.Domain.Enums;

namespace BidService.Application.DTO.Responses
{
    public class BidResponseModel
    {
        public Guid Id { get; set; }
        public Guid RFQId { get; set; }
        public Guid UserId { get; set; }
        public string Proposal { get; set; }
        public decimal CostOfProduct { get; set; }
        public decimal CostOfShipping { get; set; }
        public decimal Discount { get; set; }
        public DateTime SubmissionDate {  get; set; }
        public BidStatus BidStatus { get; set; }
        public FileResponse CompanyProfile { get; set; }
        public FileResponse ProjectPlan { get; set; }
        public FileResponse ProposalFiles { get; set; }
    }
}

    public class FileResponse
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
    }

