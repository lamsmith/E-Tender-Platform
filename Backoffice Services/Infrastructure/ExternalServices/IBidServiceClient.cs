using Backoffice_Services.Application.DTO.BidManagement.Common;
using Backoffice_Services.Application.DTO.BidManagement.Responses;
using Backoffice_Services.Application.Features.BidManagement.Commands;
using RFQService.Domain.Paging;

namespace Backoffice_Services.Infrastructure.ExternalServices
{
    public interface IBidServiceClient
    {
        Task<PaginatedList<BidResponseModel>> GetBidsAsync(
            Guid? rfqId,
            Guid? bidderId,
            BidStatus? status,
            DateTime? fromDate,
            DateTime? toDate,
            PageRequest pageRequest);

        Task<BidResponseModel> SubmitBidAsync(SubmitBidCommand request);
        Task<BidResponseModel> GetBidByIdAsync(Guid bidId);
        Task<bool> UpdateBidStatusAsync(Guid bidId, BidStatus status, string? notes);
    }
}