namespace Hospital_Management_System.Contracts.Authorization;

public record AuthorResponse
(
    string Id,
    string FirstName,
    string LastName,
    string? Email,
    string UserName,
    string Token,
    int ExpiresIn,
    string RefreshToken,
    DateTime RefreshTokenExpiration
);
