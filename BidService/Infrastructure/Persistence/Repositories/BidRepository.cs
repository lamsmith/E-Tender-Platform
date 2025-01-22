using BidService.Application.Common;
using BidService.Domain.Entities;
using BidService.Domain.Enums;
using BidService.Domain.Paging;
using BidService.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace BidService.Infrastructure.Persistence.Repositories
{
    public class BidRepository : IBidRepository
    {
        private readonly BidDbContext _context;
        private readonly IDistributedCache _cache;

        public BidRepository(BidDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Bid> AddAsync(Bid bid)
        {
            var addedBid = await _context.Bids.AddAsync(bid);
            await _context.SaveChangesAsync();
            await CacheBidAsync(addedBid.Entity);
            return addedBid.Entity;
        }

        public async Task<Bid> UpdateAsync(Bid bid)
        {
            _context.Bids.Update(bid);
            await _context.SaveChangesAsync();
            await CacheBidAsync(bid);
            await RemoveCachedBidAsync(bid.Id); // Clear old cache
            return bid;
        }

        public async Task<Bid> GetByIdAsync(Guid id)
        {
            // First check cache
            var cachedBid = await GetCachedBidAsync(id);
            if (cachedBid != null) return cachedBid;

            // If not in cache, get from DB and cache it
            var bid = await _context.Bids
                .Include(b => b.ProposalFiles)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bid != null) await CacheBidAsync(bid);
            return bid;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var bid = await _context.Bids.FindAsync(id);
            if (bid == null) return false;

            _context.Bids.Remove(bid);
            var deleted = await _context.SaveChangesAsync() > 0;
            if (deleted) await RemoveCachedBidAsync(id);
            return deleted;
        }

        public async Task<PaginatedList<Bid>> GetBidsByRFQAsync(Guid rfqId, PageRequest pageRequest)
        {
            var query = _context.Bids.Where(b => b.RFQId == rfqId);

            if (!string.IsNullOrEmpty(pageRequest.SortBy))
            {
                query = pageRequest.IsAscending
                    ? query.OrderBy(b => EF.Property<object>(b, pageRequest.SortBy))
                    : query.OrderByDescending(b => EF.Property<object>(b, pageRequest.SortBy));
            }

            // Apply filtering if a keyword is provided
            if (!string.IsNullOrEmpty(pageRequest.Keyword))
            {
                query = query.Where(b => b.Amount.ToString().Contains(pageRequest.Keyword)); // Example filtering, adjust as needed
            }

            var totalItems = await query.CountAsync();

            if (pageRequest.UsePaging)
            {
                query = query
                    .Skip((pageRequest.Page - 1) * pageRequest.PageSize)
                    .Take(pageRequest.PageSize);
            }

            var bids = await query.ToListAsync();
            return bids.ToPaginatedList(totalItems, pageRequest.Page, pageRequest.PageSize);
        }

        public async Task<PaginatedList<Bid>> CountBidsByUserAsync(Guid userId, PageRequest pageRequest)
        {
            var query = _context.Bids
                .Where(b => b.UserId == userId)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(pageRequest.SortBy))
            {
                query = pageRequest.IsAscending
                    ? query.OrderBy(b => EF.Property<object>(b, pageRequest.SortBy))
                    : query.OrderByDescending(b => EF.Property<object>(b, pageRequest.SortBy));
            }

            if (!string.IsNullOrEmpty(pageRequest.Keyword))
            {
                query = query.Where(b => b.ToString().Contains(pageRequest.Keyword));
            }

            var totalItems = await query.LongCountAsync(); 

            var items = query
                .Skip((pageRequest.Page - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize);

            var bids = await items.ToListAsync();

            return bids.ToPaginatedList(totalItems, pageRequest.Page, pageRequest.PageSize);
        }

        public async Task<(int TotalBids, int SuccessfulBids)> GetBidStatsByUserAsync(Guid userId)
        {
            var stats = await _context.Bids
                .Where(b => b.UserId == userId)
                .GroupBy(b => 1) // Grouping by a constant to aggregate all bids for the user
                .Select(g => new
                {
                    TotalBids = g.Count(),
                    SuccessfulBids = g.Count(b => b.Status == BidStatus.Accepted)
                })
                .FirstOrDefaultAsync();

            if (stats == null) return (0, 0);
            return (stats.TotalBids, stats.SuccessfulBids);
        }

        public async Task<int> CountBidsByStatusAsync(BidStatus status)
        {
            return await _context.Bids.CountAsync(b => b.Status == status);
        }





        public async Task CacheBidAsync(Bid bid)
        {
            var cacheEntryOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)); // Cache for 30 minutes, adjust as needed

            var serializedBid = JsonConvert.SerializeObject(bid);
            await _cache.SetStringAsync($"bid:{bid.Id}", serializedBid, cacheEntryOptions);
        }

        public async Task<Bid> GetCachedBidAsync(Guid id)
        {
            var cachedString = await _cache.GetStringAsync($"bid:{id}");
            if (string.IsNullOrEmpty(cachedString)) return null;
            return JsonConvert.DeserializeObject<Bid>(cachedString);
        }

        public async Task RemoveCachedBidAsync(Guid id)
        {
            await _cache.RemoveAsync($"bid:{id}");
        }
    }
}
