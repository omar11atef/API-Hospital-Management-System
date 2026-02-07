namespace Hospital_Management_System.Authentication;

public interface IJwtProvider
{
    (string token, int expiry) GentrateJwtToken(ApplicationUser applicationUser);
}
