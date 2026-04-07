using System.Data;

namespace Hospital_Management_System.Authentication;

public interface IJwtProvider
{
    (string token, int expiry) GentrateJwtToken(ApplicationUser applicationUser,IEnumerable<string> roles, IEnumerable<string> persmissions);
    string? ValidationToken(string token);
}
