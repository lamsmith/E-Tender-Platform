using Backoffice_Services.Domain.Entities;
using Backoffice_Services.Domain.Enums;

namespace Backoffice_Services.Application.Common.Interfaces
{
    public interface IStaffRepository
    {
        Task<Staff> GetByIdAsync(Guid id);
        Task<Staff> GetByUserIdAsync(Guid userId);
        Task<Staff> GetByEmailAsync(string email);
        Task<List<Staff>> GetAllAsync();
        Task<Staff> CreateAsync(Staff staff);
        Task UpdateAsync(Staff staff);
        Task DeleteAsync(Guid id);
        Task<bool> HasPermissionAsync(Guid staffId, PermissionType permission);
        Task<List<PermissionType>> GetPermissionsAsync(Guid staffId);
        Task UpdatePermissionsAsync(Guid staffId, List<PermissionType> permissions);
    }
}