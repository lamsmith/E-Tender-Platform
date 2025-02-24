using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RFQService.Application.Common.Interface.Repositories;
using RFQService.Domain.Entities;
using RFQService.Domain.Enums;
using RFQService.Domain.Paging;
using RFQService.Infrastructure.Persistence.Context;
using StackExchange.Redis;

namespace RFQService.Infrastructure.Persistence.Repositories
{
    public class RFQRepository : IRFQRepository
    {
        private readonly RFQDbContext _context;
        private readonly IDistributedCache _cache;
        private ILogger <RFQRepository> _logger;

        public RFQRepository(RFQDbContext context, IDistributedCache cache, ILogger<RFQRepository> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<RFQ> AddAsync(RFQ rfq)
        {
            var addedRfq = await _context.RFQs.AddAsync(rfq);
            await _context.SaveChangesAsync();

            try
            {
                await CacheRFQAsync(addedRfq.Entity);
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogError("Redis connection error: {Message}", ex.Message);
              
            }

            return addedRfq.Entity;
        }

        public async Task<RFQ> UpdateAsync(RFQ rfq)
        {
            _context.RFQs.Update(rfq);
            await _context.SaveChangesAsync();
            await CacheRFQAsync(rfq);
            await RemoveCachedRFQAsync(rfq.Id); 
            return rfq;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var rfq = await _context.RFQs.FindAsync(id);
            if (rfq == null) return false;

            _context.RFQs.Remove(rfq);
            var deleted = await _context.SaveChangesAsync() > 0;
            if (deleted) await RemoveCachedRFQAsync(id);
            return deleted;
        }

        public async Task<RFQ> GetByIdAsync(Guid id)
        {
            try
            {
                
                var cachedRfq = await GetCachedRFQAsync(id);
                if (cachedRfq != null) return cachedRfq;

                
                var rfq = await _context.RFQs
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (rfq != null)
                {
                    await CacheRFQAsync(rfq);
                    return rfq;
                }

                return null; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving RFQ with ID: {Id}", id);
                throw;
            }
        }

        public async Task<PaginatedList<RFQ>> GetAllAsync(PageRequest pageRequest, bool usePaging = true)
        {
            var query = _context.RFQs.AsQueryable();

            // Apply sorting if SortBy is provided
            if (!string.IsNullOrEmpty(pageRequest.SortBy))
            {
                query = pageRequest.IsAscending
                    ? query.OrderBy(r => EF.Property<object>(r, pageRequest.SortBy))
                    : query.OrderByDescending(r => EF.Property<object>(r, pageRequest.SortBy));
            }

            // Apply filtering if a keyword is provided
            if (!string.IsNullOrEmpty(pageRequest.Keyword))
            {
                query = query.Where(r => r.ContractTitle.Contains(pageRequest.Keyword) ||
                                         r.ScopeOfSupply.Contains(pageRequest.Keyword));
            }

            // Get total count before applying paging
            var totalItems = await query.CountAsync();

            if (usePaging)
            {
                query = query
                    .Skip((pageRequest.Page - 1) * pageRequest.PageSize)
                    .Take(pageRequest.PageSize);
            }

            var rfqs = await query.ToListAsync();

            return rfqs.ToPaginatedList(totalItems, pageRequest.Page, pageRequest.PageSize);
        }


        public async Task<IEnumerable<RFQ>> GetByVisibilityAsync(VisibilityType visibility)
        {
            return await _context.RFQs.Where(r => r.Visibility == visibility).ToListAsync();
        }

        public async Task<int> CountByCreatorAsync(Guid creatorId)
        {
            return await _context.RFQs.CountAsync(r => r.CreatedByUserId == creatorId);
        }

        public async Task<int> CountByStatusAsync(Status status)
        {
            return await _context.RFQs.CountAsync(r => r.Status == status);
        }


        public async Task CacheRFQAsync(RFQ rfq)
        {
            if (rfq == null)
            {
                _logger.LogError("RFQ is null, skipping caching.");
                return;
            }

            if (rfq.Id == Guid.Empty)
            {
                _logger.LogError("RFQ ID is empty, skipping caching.");
                return;
            }

            try
            {
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

                var serializedRfq = JsonConvert.SerializeObject(rfq, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                await _cache.SetStringAsync($"rfq:{rfq.Id}", serializedRfq, cacheEntryOptions);

                _logger.LogInformation("Successfully cached RFQ ID: {RfqId}", rfq.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serializing or caching RFQ ID: {RfqId}", rfq.Id);
                throw;
            }
        }


        public async Task<RFQ> GetCachedRFQAsync(Guid id)
        {
            var cachedString = await _cache.GetStringAsync($"rfq:{id}");
            if (string.IsNullOrEmpty(cachedString)) return null;
            return JsonConvert.DeserializeObject<RFQ>(cachedString);
        }

        public async Task RemoveCachedRFQAsync(Guid id)
        {
            await _cache.RemoveAsync($"rfq:{id}");
        }
    }
}
