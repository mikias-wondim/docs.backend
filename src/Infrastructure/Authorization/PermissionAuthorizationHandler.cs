using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using static System.Guid;

namespace Infrastructure.Authorization;

internal sealed class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // 1) Must be authenticated
        if (context.User.Identity is not { IsAuthenticated: true })
        {
            context.Fail();
            return;
        }

        // 2) Must have a valid GUID in NameIdentifier
        Guid userId;
        try
        {
            userId = context.User.GetUserId();
            if (userId == Empty)
            {
                context.Fail();
                return;
            }
        }
        catch (Exception)
        {
            context.Fail();
            return;
        }

        using IServiceScope scope = serviceScopeFactory.CreateScope();

        PermissionProvider permissionProvider = scope.ServiceProvider.GetRequiredService<PermissionProvider>();

        HashSet<string> permissions = await permissionProvider.GetForUserIdAsync(userId);
        
        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
