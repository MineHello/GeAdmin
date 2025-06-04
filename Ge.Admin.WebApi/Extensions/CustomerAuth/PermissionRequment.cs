using Microsoft.AspNetCore.Authorization;

namespace Ge.Admin.WebApi.Extensions.CustomerAuth
{
    public class PermissionRequment : AuthorizationHandler<PermissionRequment>, IAuthorizationRequirement
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequment requirement)
        {
            await Task.CompletedTask;
            context.Succeed(requirement);
            return;
        }
    }
}
