using Hospital_Management_System.Authentication;
using Hospital_Management_System.Errors;

namespace Hospital_Management_System.Services;

public class AuthorService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthorService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    public async Task<Result<AuthorResponse>> IsAuthorCorrectAysun(string email, string password, CancellationTokenSource cancellationTokenSource)
    {
        // check Email :
        var userByEmail = await _userManager.FindByEmailAsync(email);
        if (userByEmail == null)
            return Result.Failure<AuthorResponse>(UserErrors.InvalidCredentials);
        //check Password :
        var isPasswordValid = await _userManager.CheckPasswordAsync(userByEmail, password); 
        if (!isPasswordValid)
            return Result.Failure<AuthorResponse>(UserErrors.InvalidCredentials);
        var (token, expiry) = _jwtProvider.GentrateJwtToken(userByEmail);

        var result = new AuthorResponse
        (
            Id: userByEmail.Id,
            FirstName: userByEmail.FirstName,
            LastName: userByEmail.LastName,
            Email: userByEmail.Email,
            Token: token,
            expiration: expiry
        );
        return Result.Success(result);
    }
}
