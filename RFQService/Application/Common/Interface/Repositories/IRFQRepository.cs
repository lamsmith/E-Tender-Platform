using RFQService.Domain.Entities;
using RFQService.Domain.Enums;
using RFQService.Domain.Paging;

namespace RFQService.Application.Common.Interface.Repositories
{
    public interface IRFQRepository
    {
        Task<RFQ> AddAsync(RFQ rfq);
        Task<RFQ> UpdateAsync(RFQ rfq);
        Task<bool> DeleteAsync(Guid id);
        Task<RFQ> GetByIdAsync(Guid id);
        Task<PaginatedList<RFQ>> GetAllAsync(PageRequest pageRequest, bool usePaging = true);
        Task<IEnumerable<RFQ>> GetByVisibilityAsync(VisibilityType visibility);
        Task<int> CountByCreatorAsync(Guid creatorId);
        Task<int> CountByStatusAsync(Status status);
       

        Task CacheRFQAsync(RFQ rfq);
        Task<RFQ> GetCachedRFQAsync(Guid id);
        Task RemoveCachedRFQAsync(Guid id);
    }
}
