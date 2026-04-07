using Hospital_Management_System.Abstractions.Consts;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;


namespace Hospital_Management_System.Authentication;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;
    public (string token, int expiry) GentrateJwtToken(ApplicationUser applicationUser , IEnumerable<string> roles,IEnumerable<string> permissions)
    {
        // Cliams
        var claims = new[]
        {
            //new Claim(JwtRegisteredClaimNames.Sub, applicationUser.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, applicationUser.Id),
            new Claim(ClaimTypes.Name, applicationUser.UserName!),
            new (nameof(roles),JsonSerializer.Serialize(roles),JsonClaimValueTypes.JsonArray),
            new (nameof(permissions),JsonSerializer.Serialize(permissions),JsonClaimValueTypes.JsonArray),
        };
       

        // Key 
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.key));
        // Credentials
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        // Expiry
        //var expity = 30;
        //Token

        var token = new JwtSecurityToken(
            issuer: _options.issuer,
            audience: _options.audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_options.expires),
            signingCredentials: cred
            );
        // return 
        return (new JwtSecurityTokenHandler().WriteToken(token), _options.expires * 60);
    }


    public string? ValidationToken(string token)
    {
        // Take Security Token :
        var tokenHander = new JwtSecurityTokenHandler();
        // Key Token 
        var symmetricSecurityHander = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.key));
        // Start Validation that Token == refersh Token 
        try
        {
            tokenHander.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = symmetricSecurityHander,
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ClockSkew = TimeSpan.Zero,
            }, out SecurityToken securityToken);
            var jwtToekn = (JwtSecurityToken)securityToken;
            return jwtToekn.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
        }
        catch
        {
            return null;
        }
    }
}
