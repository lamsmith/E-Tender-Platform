using BidService.Domain.Enums;

namespace BidService.Application.DTO.Responses
{
    public class BidResponseModel
    {
        public Guid Id { get; set; }
        public Guid RFQId { get; set; }
        public Guid UserId { get; set; }
        public decimal BidAmount { get; set; }
        public BidStatus BidStatus { get; set; }
        public List<ProposalFileResponseModel> ProposalFiles { get; set; }
    }

    public class ProposalFileResponseModel
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
    }
}
