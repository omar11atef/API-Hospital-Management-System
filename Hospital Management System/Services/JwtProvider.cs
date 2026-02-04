
namespace Hospital_Management_System.Services;

public class JwtProvider : IJwtProvider
{
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
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("HQ+xNCxTvq4QcdlfPcWbjQxsMq#/lXGvdW7/h/P5vjM="));
        // Credentials
        var cred  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        // Expiry
        var expity = 30;
        //Token

        var token =new JwtSecurityToken(
            issuer: "HospitalManagementSystem",
            audience: "HospitalManagementSystemClient",
            claims: claims,
            expires: DateTime.Now.AddMinutes(expity),
            signingCredentials: cred
            );
        // return 
        return (new JwtSecurityTokenHandler().WriteToken(token), expity * 60);
    }
}
