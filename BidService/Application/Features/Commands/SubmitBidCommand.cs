using BidService.Application.DTO.Requests;
using BidService.Domain.Entities;
using MediatR;

namespace BidService.Application.Features.Commands
{
    public class SubmitBidCommand : IRequest<Bid>
    {
        public BidCreationRequestModel BidData { get; set; }
    }
}
