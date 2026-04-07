namespace Hospital_Management_System.Authentication.Filter;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var user = context.User.Identity;
        if (user is null||!user.IsAuthenticated)
            return;
        var hasPermission = context.User.Claims.Any(x => x.Value == requirement.Permission && x.Type == Permissions.Type);
        if (!hasPermission)
            return;
        // User has that cliam :
        context.Succeed(requirement);
        return;

    }
}
