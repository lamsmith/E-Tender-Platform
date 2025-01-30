using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Backoffice_Services.Domain.Enums;

namespace Backoffice_Services.Infrastructure.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var permissions = context.User.Claims
                .Where(x => x.Type == "Permission")
                .Select(x => Enum.Parse<PermissionType>(x.Value));

            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionType Permission { get; }

        public PermissionRequirement(PermissionType permission)
        {
            Permission = permission;
        }
    }
}