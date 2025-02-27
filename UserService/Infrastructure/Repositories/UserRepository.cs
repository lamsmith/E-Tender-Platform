using MassTransit;
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
                    u.Profile.State.Contains(pageRequest.Keyword) ||
                    u.Profile.CompanyAddress.Contains(pageRequest.Keyword) ||
                    u.Profile.Industry.Contains(pageRequest.Keyword) ||
                    u.Profile.City.Contains(pageRequest.Keyword));
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

        public async Task<(string FirstName, string LastName)?> GetUserNamesByIdAsync(Guid userId)
        {
            var result = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new { u.Profile.FirstName, u.Profile.LastName })
                .FirstOrDefaultAsync();

            return result == null ? null : (result.FirstName, result.LastName);
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

        public async Task<int> GetCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        //public async Task<int> CountActiveUsersAsync()
        //{
        //    return await _context.Users.CountAsync(u => u.LastLogin >= DateTime.UtcNow.AddDays(-30));
        //}

        public async Task<PaginatedList<User>> GetIncompleteProfilesAsync(PageRequest pageRequest)
        {
            IQueryable<User> query = _context.Users
                .Include(u => u.Profile)
                .Where(u =>
                    u.Profile == null ||
                    string.IsNullOrEmpty(u.Profile.CompanyName) ||
                    string.IsNullOrEmpty(u.Profile.RcNumber) ||
                    string.IsNullOrEmpty(u.Profile.CompanyAddress) ||
                    string.IsNullOrEmpty(u.Profile.PhoneNumber) ||
                    string.IsNullOrEmpty(u.Profile.Industry) ||
                    string.IsNullOrEmpty(u.Profile.State) ||
                    string.IsNullOrEmpty(u.Profile.City)
                );

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(pageRequest.SortBy))
            {
                query = pageRequest.IsAscending
                    ? query.OrderBy(u => EF.Property<object>(u, pageRequest.SortBy))
                    : query.OrderByDescending(u => EF.Property<object>(u, pageRequest.SortBy));
            }
            else
            {
                // Default sorting by creation date
                query = query.OrderByDescending(u => u.CreatedAt);
            }

            // Get total count
            var totalItems = await query.LongCountAsync();

            // Apply paging
            var users = await query
                .Skip((pageRequest.Page - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .ToListAsync();

            return new PaginatedList<User>
            {
                Items = users,
                TotalItems = totalItems,
                Page = pageRequest.Page,
                PageSize = pageRequest.PageSize
            };
        }

    }
}
