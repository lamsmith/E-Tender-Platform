using Microsoft.EntityFrameworkCore;
using Backoffice_Services.Application.Common.Interfaces;
using Backoffice_Services.Domain.Entities;
using Backoffice_Services.Domain.Enums;
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
                .Include(s => s.Permissions)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Staff> GetByUserIdAsync(Guid userId)
        {
            return await _context.Staff
                .Include(s => s.Permissions)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<Staff> GetByEmailAsync(string email)
        {
            return await _context.Staff
                .Include(s => s.Permissions)
                .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());
        }

        public async Task<List<Staff>> GetAllAsync()
        {
            return await _context.Staff
                .Include(s => s.Permissions)
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

        public async Task<bool> HasPermissionAsync(Guid staffId, PermissionType permission)
        {
            return await _context.StaffPermissions
                .AnyAsync(p => p.StaffId == staffId &&
                              p.PermissionType == permission &&
                              p.IsGranted);
        }

        public async Task<List<PermissionType>> GetPermissionsAsync(Guid staffId)
        {
            return await _context.StaffPermissions
                .Where(p => p.StaffId == staffId && p.IsGranted)
                .Select(p => p.PermissionType)
                .ToListAsync();
        }

        public async Task UpdatePermissionsAsync(Guid staffId, List<PermissionType> permissions)
        {
            var existingPermissions = await _context.StaffPermissions
                .Where(p => p.StaffId == staffId)
                .ToListAsync();

            _context.StaffPermissions.RemoveRange(existingPermissions);

            var newPermissions = permissions.Select(p => new StaffPermission
            {
                StaffId = staffId,
                PermissionType = p,
                IsGranted = true
            });

            await _context.StaffPermissions.AddRangeAsync(newPermissions);
            await _context.SaveChangesAsync();
        }
    }
}