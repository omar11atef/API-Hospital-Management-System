namespace Hospital_Management_System.Contracts.User;

public record UserProfileResponse
(
    string Email,
    string UserName,
    string FirstName,
    string LastName
);
