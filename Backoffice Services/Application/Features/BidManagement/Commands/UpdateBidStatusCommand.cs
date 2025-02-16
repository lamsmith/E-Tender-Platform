using Backoffice_Services.Application.DTO.BidManagement.Common;
using MediatR;

namespace Backoffice_Services.Application.Features.BidManagement.Commands
{
    public class UpdateBidStatusCommand : IRequest<bool>
    {
        public Guid BidId { get; set; }
        public BidStatus Status { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public string? Notes { get; set; }
    }
}