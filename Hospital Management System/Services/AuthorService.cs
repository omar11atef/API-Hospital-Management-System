using Hospital_Management_System.Authentication;

namespace Hospital_Management_System.Services;

public class AuthorService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthorService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    public async Task<AuthorResponse?> IsAuthorCorrectAysun(string email, string password, CancellationTokenSource cancellationTokenSource)
    {
        // check Email :
        var userByEmail = await _userManager.FindByEmailAsync(email);
        if (userByEmail == null)
            return null;
        //check Password :
        var isPasswordValid = await _userManager.CheckPasswordAsync(userByEmail, password); 
        if (!isPasswordValid)
            return null;
        var (token, expiry) = _jwtProvider.GentrateJwtToken(userByEmail);

        return new AuthorResponse
        (
            Id: userByEmail.Id,
            FirstName: userByEmail.FirstName,
            LastName: userByEmail.LastName,
            Email: userByEmail.Email,
            Token: token,
            expiration: expiry
        );
    }
}
