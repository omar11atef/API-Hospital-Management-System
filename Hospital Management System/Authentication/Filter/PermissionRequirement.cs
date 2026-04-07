namespace Hospital_Management_System.Authentication.Filter;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
