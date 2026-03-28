using System.Security.Cryptography;

namespace Hospital_Management_System.Services;

public class AuthorService(UserManager<ApplicationUser> userManager
    , IJwtProvider jwtProvider, SignInManager<ApplicationUser> signInManager
    , IEmailSender emailSender
    , ILogger<AuthorService> logger
    , IHttpContextAccessor httpContextAccessor ) : IAuthorService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<AuthorService> _logger = logger;

    private readonly int _refershTokenDays = 14;
    public async Task<Result<AuthorResponse>> IsAuthorCorrectAysun(string email, string password, CancellationTokenSource cancellationTokenSource)
    {
        // check Email :
        var userByEmail = await _userManager.FindByEmailAsync(email);
        if (userByEmail == null)
            return Result.Failure<AuthorResponse>(UserErrors.InvalidCredentials);
        //check Password :
        var result = await _signInManager.CheckPasswordSignInAsync(userByEmail, password, false);

        if (result.Succeeded)
        {
            var respone = await TokenGenerateAsync(userByEmail);

            return Result.Success(respone);
        }
        return Result.Failure<AuthorResponse>(result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentials);
    }

    public async Task<Result> RegisterAsync (RegisterRequestApp request , CancellationToken cancellationToken)
    {
        //check that email find or not :
        var EmailExit = await _userManager.Users.AnyAsync(x => x.Email == request.Email);
        if (EmailExit)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            return existingUser!.EmailConfirmed
                ? Result.Failure(UserErrors.EmailAlreadyConfirm)
                : Result.Failure(UserErrors.EmailRegisteredButNotConfirmed);
        }

        var existingUserName = await _userManager.FindByNameAsync(request.UserName);
        if (existingUserName is not null)
            return Result.Failure<string>(UserErrors.UserNameAlreadyExists);
       
        // not Exit --> make register that email 
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };
        var result = await _userManager.CreateAsync(user,request.Password);
        if (result.Succeeded)
        { 
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _logger.LogInformation("Code for Confirmations:{code}", code);

            await SendConfirmationMail(user, code);

            return Result.Success(code);
        }
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description));

    }
    // Confirmation by 64UrlDecode :
    /*public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var EmailExit = await _userManager.FindByEmailAsync(request.UserId);
        if (EmailExit is null)
            return Result.Failure(UserErrors.EmailAlreadyConfirm);
        if (EmailExit.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirm);
        // Email not Confirmation --> send Code :
        var code = request.Code;
        try { code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)); }
        catch (FormatException) { return Result.Failure<AuthorResponse>(UserErrors.InvalidCode); }
        // code Correct Decoding :
        var result = await _userManager.ConfirmEmailAsync(EmailExit, code);
        if (result.Succeeded)
        {
            _logger.LogInformation("Code for Confirmations:{code}", code);
            return Result.Success(code);
        };

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description));
    }*/

    // Confirmation By 6 numbers :
    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);

        // 2. Check if already confirmed
        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirm);

        // 3. The 6-digit code typed by the user
        var code = request.Code ;
        try { code = request.Code; }
        catch (FormatException) { return Result.Failure<AuthorResponse>(UserErrors.InvalidCode); }

        // 4. Confirm the email directly! (No Base64 decoding needed)
        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            _logger.LogInformation("Email confirmed successfully for user {Email}. Code used: {Code}", user.Email, code);
            await SendConfirmationMail(user, code);
            return Result.Success();
        }

        // 5. If the code is wrong or expired
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description));
    }

    // Resend Confiramtion Email :
    public async Task<Result> ResendConfirmEmailAsync(ResendConfirmEmailRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirm);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        _logger.LogInformation("ResendConfirmEmail: A new 6-digit code {Code} was generated for {Email}", code, user.Email);
        await SendConfirmationMail(user, code);
        // 4. إرسال الكود عبر خدمة الإيميل
        // await _emailService.SendEmailAsync(user.Email, "Your New Confirmation Code", $"Your new code is: {code}");

        return Result.Success();
    }


    // Private methods :
    private static string RefershTokenGenertion()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private async Task<AuthorResponse> TokenGenerateAsync(ApplicationUser user)
    {
        var (token, expriation) = _jwtProvider.GentrateJwtToken(user);
        // Add RefreshToken :

        var refreshToken = RefershTokenGenertion();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refershTokenDays);
        user.RefreshTokens.Add(new RefreshTokens
        {
            Token = refreshToken,
            ExpiresON = refreshTokenExpiration,
        });
        await _userManager.UpdateAsync(user);
        var respone = new AuthorResponse(
            Id: user.Id,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Token: _jwtProvider.GentrateJwtToken(user).token,
            ExpiresIn: _jwtProvider.GentrateJwtToken(user).expiry,
            RefreshToken: refreshToken,
            RefreshTokenExpiration: refreshTokenExpiration);
        return respone;
    }

    private async Task SendConfirmationMail(ApplicationUser user, string code)
    {
        var orgin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        // Call Generate Email Body 
        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailComfirmation",
            new Dictionary<string, string>
        {
                {"{{name}}",user.FirstName},
                { "{{action_url}}",$"{orgin}/auth/emailConfiramtion?userId={user.Id}&code={code}"},
                { "{{code}}",$"code={code}"}
        });
        await _emailSender.SendEmailAsync(user.Email!, "Valerio: Email Confirmation", emailBody);

    }


}
