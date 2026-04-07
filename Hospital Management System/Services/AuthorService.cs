using System.Data;
using System.Security.Cryptography;


namespace Hospital_Management_System.Services;

public class AuthorService(UserManager<ApplicationUser> userManager
    , IJwtProvider jwtProvider, SignInManager<ApplicationUser> signInManager
    , IEmailSender emailSender
    , ILogger<AuthorService> logger
    , IHttpContextAccessor httpContextAccessor,
    ApplicationDbContext context) : IAuthorService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<AuthorService> _logger = logger;
    private readonly ApplicationDbContext _context = context;


    private readonly int _refershTokenDays = 14;
    public async Task<Result<AuthorResponse>> IsAuthorCorrectAysun(string email, string password, CancellationToken cancellationToken)
    {
        // check Email :
        var userByEmail = await _userManager.FindByEmailAsync(email);
        if (userByEmail == null)
            return Result.Failure<AuthorResponse>(UserErrors.InvalidCredentials);
        //Check that User IsDisable or not :
        if(userByEmail.IsDisable)
            return Result.Failure<AuthorResponse>(UserErrors.DisableUser);

        //check Password :
        //var result = await _signInManager.CheckPasswordSignInAsync(userByEmail, password, true);
        var result = await _signInManager.PasswordSignInAsync(userByEmail, password,false, true);

        // Login and Generation Token :
        if (result.Succeeded)
        {
            // Private Method to roles and claims :
            var (UserRoles, UserCliams) = await GetUserRolesAndPermission(userByEmail, cancellationToken);
            // Private Method get Token :
            var respone = await TokenGenerateAsync(userByEmail,cancellationToken);

            await _signInManager.SignInAsync(userByEmail, isPersistent: true);
            return Result.Success(respone);
        }

        var error = result.IsNotAllowed
            ? UserErrors.EmailNotConfirmed
            : result.IsLockedOut
                ? UserErrors.LockedUser
                : UserErrors.InvalidCredentials;

        return Result.Failure<AuthorResponse>(error);
    }

    // Register :
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
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

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
            // _logger.LogInformation("Email confirmed successfully for user {Email}. Code used: {Code}", user.Email, code);
            // await SendConfirmationMail(user, code);

            // Add User in Role (By Default Memeber)
            await _userManager.AddToRoleAsync(user,DefaultRoles.Member);
            return Result.Success();
        }

        // 5. If the code is wrong or expired
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
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

    //Forgate Password :
    public async Task<Result> SendResetPasswordCodeAsync(string email, CancellationToken cancellationToken = default)
    {
        // User With that email or not :
        if(await _userManager.FindByEmailAsync(email) is not { }user)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        // Find that user :
        // 1- Generation code 
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        // 2- Send Password :
        _logger.LogInformation("Resend Password: A new code {Code} was resent for {passwrod}", code, user.Email);
        await SendResetPasswordMail(user, code);
        
        return Result.Success();
    }

    //  Reset new Password :
    public async Task<Result> ResetPasswordAsync(ResendPasswordRequestApp request)
    {
        // Select User :
        var user =await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCode);

        IdentityResult result;
        try
        {
            //var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result = await _userManager.ResetPasswordAsync(user, request.Code, request.NewPassword);
        }
        catch(FormatException )
        {
            result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
        }
        if(result.Succeeded)
            return Result.Success();
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }

    // Get Refresh Token :
    public async Task<Result<AuthorResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidationToken(token);

        if (userId is null)
            return Result.Failure<AuthorResponse>(UserErrors.InvalidJwtToken);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure<AuthorResponse>(UserErrors.InvalidJwtToken);

        if(user.IsDisable)
            return Result.Failure<AuthorResponse>(UserErrors.DisableUser);

        if (user.LockoutEnd > DateTime.UtcNow)
            return Result.Failure<AuthorResponse>(UserErrors.LockedUser);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthorResponse>(UserErrors.InvalidRefreshToken);

        userRefreshToken.ReokedON = DateTime.UtcNow;

        var (userRoles, userPermissions) = await GetUserRolesAndPermission(user, cancellationToken);

        var (newToken, expiresIn) = _jwtProvider.GentrateJwtToken(user, userRoles, userPermissions);
        var newRefreshToken = RefershTokenGenertion();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refershTokenDays);

        user.RefreshTokens.Add(new RefreshTokens
        {
            Token = newRefreshToken,
            ExpiresON = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthorResponse(user.Id, user.FirstName, user.LastName, user.Email,user.UserName!, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }

    // Revoke Refresh Token :
    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidationToken(token);

        if (userId is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidRefreshToken);

        userRefreshToken.ReokedON = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }



    // Private methods :
    private static string RefershTokenGenertion()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private async Task<AuthorResponse> TokenGenerateAsync(ApplicationUser user , CancellationToken cancellationToken)
    {

        var (UserRoles, UserCliams) = await GetUserRolesAndPermission(user, cancellationToken);

        var (token, expriation) = _jwtProvider.GentrateJwtToken(user, UserRoles, UserCliams);
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
            UserName:user.UserName!,
            Token: _jwtProvider.GentrateJwtToken(user,UserRoles, UserCliams).token,
            ExpiresIn: _jwtProvider.GentrateJwtToken(user, UserRoles, UserCliams).expiry,
            RefreshToken: refreshToken,
            RefreshTokenExpiration: refreshTokenExpiration
            );
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
    private async Task SendResetPasswordMail(ApplicationUser user, string code)
    {
        var orgin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        // Call Generate Email Body 
        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailComfirmation",
            new Dictionary<string, string>
        {
                {"{{name}}",user.FirstName},
                { "{{action_url}}",$"{orgin}/auth/forgatepassword?email={user.Email}&code={code}"},
                { "{{code}}",$"code={code}"}
        });
        await _emailSender.SendEmailAsync(user.Email!, "Valerio: change password Confirmation", emailBody);

    }

    private async Task<(IEnumerable<string>roles, IEnumerable<string>permissions)> GetUserRolesAndPermission(ApplicationUser user, CancellationToken cancellationToken)
    {
        // Get Roles for User :
        var UserRoles = await _userManager.GetRolesAsync(user);
        // Get Permission for User :
        var UserCliams = await (from r in _context.Roles
                                join p in _context.RoleClaims
                                on r.Id equals p.RoleId
                                where UserRoles.Contains(r.Name!)
                                select p.ClaimValue)
                     .Distinct()
                     .ToListAsync(cancellationToken);
         
        return (UserRoles, UserCliams);
    }



}
