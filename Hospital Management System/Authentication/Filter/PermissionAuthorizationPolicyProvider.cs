
namespace Hospital_Management_System.Authentication.Filter;

public class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    private readonly AuthorizationOptions _authorizationOptions = options.Value;
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy= await base.GetPolicyAsync(policyName);
        if(policy is not null)
            return policy;

        // Policy in here is null 
        var permissionPolicy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();
        _authorizationOptions.AddPolicy(policyName, permissionPolicy);
        return permissionPolicy;
    }
}
