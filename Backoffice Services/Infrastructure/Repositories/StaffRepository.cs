using Microsoft.EntityFrameworkCore;
using Backoffice_Services.Application.Common.Interfaces;
using Backoffice_Services.Domain.Entities;
using Backoffice_Services.Infrastructure.Persistence.Context;

namespace Backoffice_Services.Infrastructure.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly BackofficeDbContext _context;
        private readonly ILogger<StaffRepository> _logger;

        public StaffRepository(
            BackofficeDbContext context,
            ILogger<StaffRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Staff> GetByIdAsync(Guid id)
        {
            return await _context.Staff
                
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Staff> GetByUserIdAsync(Guid userId)
        {
            return await _context.Staff
                
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<Staff> GetByEmailAsync(string email)
        {
            return await _context.Staff
                
                .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());
        }

        public async Task<List<Staff>> GetAllAsync()
        {
            return await _context.Staff
              
                .ToListAsync();
        }

        public async Task<Staff> CreateAsync(Staff staff)
        {
            await _context.Staff.AddAsync(staff);
            await _context.SaveChangesAsync();
            return staff;
        }

        public async Task UpdateAsync(Staff staff)
        {
            _context.Staff.Update(staff);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff != null)
            {
                _context.Staff.Remove(staff);
                await _context.SaveChangesAsync();
            }
        }

       
       
    }
}