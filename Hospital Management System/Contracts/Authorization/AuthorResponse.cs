namespace Hospital_Management_System.Contracts.Authorization;

public record AuthorResponse
(
    string Id,
    string FirstName,
    string LastName,
    string? Email,
    string Token,
    int expiration
);
