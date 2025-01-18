using UserService.Domain.Entities;
using UserService.Domain.Paging;

namespace UserService.Application.Common.Interface.Repositories
{
    public interface IUserRepository
    {
        Task<PaginatedList<User>> GetAllAsync(PageRequest pageRequest, bool usePaging = true);
        bool IsExitByEmail(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid id);
        Task<User> GetByEmailAsync(string email);


        Task<int> GetBidsSubmittedCountAsync(Guid userId);
        Task<(int TotalBids, int SuccessfulBids)> GetBidSuccessRateAsync(Guid userId);
        Task<int> GetRfqCreatedCountAsync(Guid userId);
    }
}
