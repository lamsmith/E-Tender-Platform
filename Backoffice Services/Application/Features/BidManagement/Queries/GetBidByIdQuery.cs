using Backoffice_Services.Application.DTO.BidManagement.Responses;
using MediatR;

namespace Backoffice_Services.Application.Features.BidManagement.Queries
{
    public class GetBidByIdQuery : IRequest<BidResponseModel>
    {
        public Guid Id { get; set; }
    }
}