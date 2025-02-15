using Microsoft.AspNetCore.Authorization;
using Backoffice_Services.Domain.Enums;

namespace Backoffice_Services.Infrastructure.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userPermissions = context.User.Claims
                .Where(c => c.Type == "permissions")
                .Select(c => c.Value)
                .ToList();

            if (userPermissions.Contains(requirement.Permission.ToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}