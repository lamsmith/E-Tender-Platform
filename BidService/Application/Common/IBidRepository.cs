using BidService.Domain.Entities;
using BidService.Domain.Enums;
using BidService.Domain.Paging;
using System.Threading.Tasks;

namespace BidService.Application.Common
{
    public interface IBidRepository
    {
        Task<Bid> AddAsync(Bid bid);
        Task<Bid> UpdateAsync(Bid bid);
        Task<Bid> GetByIdAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<PaginatedList<Bid>> GetBidsByRFQAsync(Guid rfqId, PageRequest pageRequest);
        Task<PaginatedList<Bid>> CountBidsByUserAsync(Guid userId, PageRequest pageRequest);
        Task<(int TotalBids, int SuccessfulBids)> GetBidStatsByUserAsync(Guid userId);
        Task<int> CountBidsByStatusAsync(BidStatus status);


        // Caching related methods
        Task CacheBidAsync(Bid bid);
        Task<Bid> GetCachedBidAsync(Guid id);
        Task RemoveCachedBidAsync(Guid id);
    }
}
