namespace Hospital_Management_System.Services;

public interface IJwtProvider
{
    (string token, int expiry) GentrateJwtToken(ApplicationUser applicationUser);
}
