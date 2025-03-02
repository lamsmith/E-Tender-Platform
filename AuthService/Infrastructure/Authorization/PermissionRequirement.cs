using Microsoft.AspNetCore.Authorization;
using AuthService.Domain.Enums;

namespace AuthService.Infrastructure.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionType Permission { get; }

        public PermissionRequirement(PermissionType permission)
        {
            Permission = permission;
        }
    }
}