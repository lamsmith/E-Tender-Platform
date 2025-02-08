using Backoffice_Services.Application.DTO.BidManagement.Common;

namespace Backoffice_Services.Application.DTO.BidManagement.Requests
{
    public class UpdateBidStatusRequest
    {
        public BidStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}