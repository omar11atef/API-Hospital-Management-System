namespace Hospital_Management_System.Contracts.Roles;

public record RoleRequest(
    string Name,
    List<string> Permissions
);
