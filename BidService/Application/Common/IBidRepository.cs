using BidService.Domain.Entities;
using BidService.Domain.Paging;

namespace BidService.Application.Common
{
    public interface IBidRepository
    {
        Task<Bid> AddAsync(Bid bid);
        Task<Bid> UpdateAsync(Bid bid);
        Task<Bid> GetByIdAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<PaginatedList<Bid>> GetBidsByRFQAsync(Guid rfqId, PageRequest pageRequest);
        Task<int> CountBidsByUserAsync(Guid userId);
        Task<(int TotalBids, int SuccessfulBids)> GetBidStatsByUserAsync(Guid userId);

        // Caching related methods
        Task CacheBidAsync(Bid bid);
        Task<Bid> GetCachedBidAsync(Guid id);
        Task RemoveCachedBidAsync(Guid id);
    }
}
