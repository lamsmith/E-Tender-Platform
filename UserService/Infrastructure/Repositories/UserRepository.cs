using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Interface.Repositories;
using UserService.Domain.Entities;
using UserService.Domain.Paging;
using UserService.Infrastructure.Persistence.Context;

namespace UserService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PaginatedList<User>> GetAllAsync(PageRequest pageRequest, bool usePaging = true)
        {
          
            // Retrieve the base query
            IQueryable<User> query = _context.Users.Include(u => u.Profile);

            // Apply filtering if a keyword is provided
            if (!string.IsNullOrWhiteSpace(pageRequest.Keyword))
            {
                query = query.Where(u =>
                    u.Profile.FirstName.Contains(pageRequest.Keyword) ||
                    u.Profile.LastName.Contains(pageRequest.Keyword) ||
                    u.Profile.PhoneNumber.Contains(pageRequest.Keyword) ||
                    u.Profile.Address.Contains(pageRequest.Keyword));
            }

            //  Apply sorting
            if (!string.IsNullOrWhiteSpace(pageRequest.SortBy))
            {
                // Use reflection or predefined mappings to apply sorting dynamically
                query = pageRequest.IsAscending
                    ? query.OrderBy(u => EF.Property<object>(u, pageRequest.SortBy))
                    : query.OrderByDescending(u => EF.Property<object>(u, pageRequest.SortBy));
            }

            // Get the total count before applying paging
            long totalItems = await query.LongCountAsync();

            //  Apply paging (if enabled)
            if (usePaging)
            {
                query = query
                    .Skip((pageRequest.Page - 1) * pageRequest.PageSize)
                    .Take(pageRequest.PageSize);
            }

            //  Execute the query and retrieve data
            List<User> users = await query.ToListAsync();

            // Return a paginated list
            return new PaginatedList<User>
            {
                Items = users,
                TotalItems = totalItems,
                Page = pageRequest.Page,
                PageSize = pageRequest.PageSize
            };
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public bool IsExitByEmail(string email) => _context.Users.Any(s => s.Email == email);

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
