using BidService.Domain.Common;
using BidService.Domain.Enums;

namespace BidService.Domain.Entities
{
    public class Bid : BaseEntity
    {
        required
        public string Proposal { get; set; }
        public decimal CostOfProduct { get; set; }
        public decimal CostOfShipping { get; set; }
        public decimal Discount  { get; set; }
        public BidStatus Status { get; set; }
        public Guid RFQId { get; set; }
        public Guid UserId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public File? CompanyProfile { get; set; }
        public File? ProjectPlan { get; set; }
        public File? ProposalFile { get; set; }
    }
}
