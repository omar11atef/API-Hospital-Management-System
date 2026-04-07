namespace Hospital_Management_System.Errors;

public static class RolesErrors
{
    public static readonly Error RoleNotFound =
        new("Role.NotFound", "That Role cannot be found.", StatusCodes.Status404NotFound);

    public static readonly Error DuplicateRole =
        new("Role.Duplicate", "Sorry: a Role with that value has already been stored.", StatusCodes.Status409Conflict);

    public static readonly Error InvalidPermission =
        new("Permissions.InvalidAmount", "Permission is invalid.", StatusCodes.Status400BadRequest);
}
