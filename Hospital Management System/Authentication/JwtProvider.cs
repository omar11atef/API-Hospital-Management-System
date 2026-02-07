using Microsoft.Extensions.Options;
using System.Text;

namespace Hospital_Management_System.Authentication;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;
    public (string token, int expiry) GentrateJwtToken(ApplicationUser applicationUser)
    {
       
        // Cliams
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, applicationUser.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, applicationUser.Id),
            new Claim(ClaimTypes.Name, applicationUser.UserName!)
        };
        // Key 
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.key));
        // Credentials
        var cred  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        // Expiry
        //var expity = 30;
        //Token

        var token =new JwtSecurityToken(
            issuer: _options.issuer,
            audience: _options.audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_options.expires),
            signingCredentials: cred
            );
        // return 
        return (new JwtSecurityTokenHandler().WriteToken(token), _options.expires * 60);
    }
}
