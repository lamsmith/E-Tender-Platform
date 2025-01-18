using BidService.Domain.Common;
using BidService.Domain.Enums;

namespace BidService.Domain.Entities
{
    public class Bid : BaseEntity
    {
        public string Proposal { get; set; }
        public decimal Amount { get; set; }  
        public BidStatus Status { get; set; }
        public Guid RFQId { get; set; }
        public Guid UserId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public ICollection<ProposalFile> ProposalFiles { get; set; } = new List<ProposalFile>();
    }
}
