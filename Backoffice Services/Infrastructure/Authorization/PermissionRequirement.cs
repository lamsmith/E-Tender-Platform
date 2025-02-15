using Microsoft.AspNetCore.Authorization;
using Backoffice_Services.Domain.Enums;

namespace Backoffice_Services.Infrastructure.Authorization
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