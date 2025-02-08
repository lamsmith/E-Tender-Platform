using BidService.Domain.Enums;

namespace BidService.Application.DTO.Requests
{
    public class BidCreationRequestModel
    {
        public Guid Id { get; set; }
        public Guid RFQId { get; set; }
        public Guid UserId { get; set; }
        public string Proposal { get; set; }
        public decimal CostOfProduct { get; set; }
        public decimal CostOfShipping { get; set; }
        public decimal Discount { get; set; }
        public BidStatus BidStatus { get; set; }
        public File CompanyProfile { get; set; }
        public File ProjectPlan { get; set; }
        public File ProposalFiles { get; set; }
    }

    public class File
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
    }
}
