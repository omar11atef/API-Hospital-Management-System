namespace Hospital_Management_System.Contracts.Authorization;

public record RefreshTokenRequest
(
    string token,
    string refreshToken
);

