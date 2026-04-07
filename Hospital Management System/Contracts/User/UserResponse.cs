namespace Hospital_Management_System.Contracts.User;

public record UserResponse(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    bool IsDisabled,
    IEnumerable<string> Roles
);



