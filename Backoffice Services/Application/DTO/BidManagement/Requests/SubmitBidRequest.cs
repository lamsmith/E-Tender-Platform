using Backoffice_Services.Application.DTO.BidManagement.Common;

namespace Backoffice_Services.Application.DTO.BidManagement.Requests
{
    public class SubmitBidRequest
    {
        public Guid RfqId { get; set; }
        public Guid UserId { get; set; }
        public string Proposal { get; set; }
        public decimal CostOfProduct { get; set; }
        public decimal CostOfShipping { get; set; }
        public decimal Discount { get; set; }
        public BidStatus Status { get; set; }
        public DateTime SubmissionDate { get; set; }
        public File? CompanyProfile { get; set; }
        public File? ProjectPlan { get; set; }
        public File? ProposalFile { get; set; }
    }

    public class File
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string FileUrl { get; set; }
    }
}