using Hospital_Management_System.Abstractions;

namespace Hospital_Management_System.Controllers;

[Route("[controller]")]
[ApiController]

public class AuthorController(IAuthorService authorService, IOptions<JwtOptions> jwtoptions, IConfiguration configuration, SignInManager<ApplicationUser> signInManager) : ControllerBase
{
    private readonly IAuthorService _authorService = authorService;
    private readonly JwtOptions _jwtoptions=jwtoptions.Value;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("Login")]
    public async Task<IActionResult> IsAuthorCorrectAysun([FromBody] LoginRequest  authorRequest, CancellationToken cancellationTokenSource)
    {
        var AuthorRuslt = await _authorService.IsAuthorCorrectAysun(authorRequest.Email, authorRequest.Password, cancellationTokenSource);

        return AuthorRuslt.IsSuccess
            ? Ok(AuthorRuslt.Value)
            : AuthorRuslt.ToProblem();
    }

    // 1. Endpoint: Register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestApp request,CancellationToken cancellationToken = default)
    {
        var result = await _authorService.RegisterAsync(request, cancellationToken);

        return result.IsSuccess? NoContent() : result.ToProblem();
    }

    // 2. Endpoint: Confirm Email
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request,CancellationToken cancellationToken = default)
    {
        var result = await _authorService.ConfirmEmailAsync(request, cancellationToken);

        return result.IsSuccess? NoContent(): result.ToProblem();
    }

    // 3. Endpoint: Resend Confirmation Email
    [HttpPost("resend-confirm-email")]
    public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendConfirmEmailRequest request,CancellationToken cancellationToken = default)
    {
        var result = await _authorService.ResendConfirmEmailAsync(request, cancellationToken);

        return result.IsSuccess
           ? NoContent()
           : result.ToProblem();
    }

    // 4. Endpoint: Resend Change Password
    [HttpPost("Resend-Change-Password")]
    public async Task<IActionResult> ResendChangePassword([FromBody] ForgatePasswrodRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _authorService.SendResetPasswordCodeAsync(request.Email, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    // 5. Reset Password :
    [HttpPost("Reset-Password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResendPasswordRequestApp request)
    {
        var result = await _authorService.ResetPasswordAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }


    // 6. Endpoint: LogOut :
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync(); // This clears the Identity Cookie
        return Ok();
    }

    


    /*[HttpGet("TestOptionPatterns")]
    public IActionResult Test()
    {
        return Ok(new { _jwtoptions.key, _jwtoptions.audience });
    }*/
}
