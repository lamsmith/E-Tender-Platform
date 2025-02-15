using Backoffice_Services.Application.DTO.BidManagement.Common;
using Backoffice_Services.Application.DTO.BidManagement.Responses;
using Backoffice_Services.Domain.Paging;
using MediatR;

namespace Backoffice_Services.Application.Features.BidManagement.Queries
{
    public class GetBidsQuery : IRequest<PaginatedList<BidResponseModel>>
    {
        public Guid? RfqId { get; set; }
        public Guid? BidderId { get; set; }
        public BidStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public PageRequest PageRequest { get; set; }
    }
}