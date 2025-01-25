using AuthService.Domain.Entities;

namespace AuthService.Application.Common.Interface.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(Guid id);
        Task UpdateAsync(User user);
        Task<bool> EmailExistsAsync(string email);
        Task DeleteAsync(Guid id);
    }
}