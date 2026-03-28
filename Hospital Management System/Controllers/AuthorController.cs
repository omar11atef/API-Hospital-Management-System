
namespace Hospital_Management_System.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthorController(IAuthorService authorService, IOptions<JwtOptions> jwtoptions, IConfiguration configuration) : ControllerBase
{
    private readonly IAuthorService _authorService = authorService;
    private readonly JwtOptions _jwtoptions=jwtoptions.Value;
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("Login")]
    public async Task<IActionResult> IsAuthorCorrectAysun([FromBody] LoginRequest  authorRequest, CancellationTokenSource cancellationTokenSource)
    {
        var AuthorRuslt = await _authorService.IsAuthorCorrectAysun(authorRequest.Email, authorRequest.Password, cancellationTokenSource);
       
        return AuthorRuslt.IsSuccess
            ? Ok(AuthorRuslt.Value)
            : BadRequest(AuthorRuslt.Error);
    }

    // 1. Endpoint: Register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestApp request,CancellationToken cancellationToken = default)
    {
        var result = await _authorService.RegisterAsync(request, cancellationToken);

        if (result.IsSuccess)
            return NoContent();
        
        return result.Error.Equals(UserErrors.EmailAlreadyConfirm) || result.Error.Equals(UserErrors.UserNameAlreadyExists)
            ? result.ToProblem(StatusCodes.Status409Conflict)
            : result.ToProblem(StatusCodes.Status400BadRequest);
    }

    // 2. Endpoint: Confirm Email
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request,CancellationToken cancellationToken = default)
    {
        var result = await _authorService.ConfirmEmailAsync(request, cancellationToken);

        if (result.IsSuccess)
            return NoContent();
        

        return result.Error.Equals(UserErrors.UserNotFound)
            ? result.ToProblem(StatusCodes.Status404NotFound)
            : result.Error.Equals(UserErrors.EmailAlreadyConfirm)
                ? result.ToProblem(StatusCodes.Status409Conflict)
                : result.ToProblem(StatusCodes.Status400BadRequest);
    }

    // 3. Endpoint: Resend Confirmation Email
    [HttpPost("resend-confirm-email")]
    public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendConfirmEmailRequest request,CancellationToken cancellationToken = default)
    {
        var result = await _authorService.ResendConfirmEmailAsync(request, cancellationToken);

        if (result.IsSuccess)
            return NoContent();
        
        return result.Error.Equals(UserErrors.EmailAlreadyConfirm)
            ? result.ToProblem(StatusCodes.Status409Conflict)
            : result.ToProblem(StatusCodes.Status400BadRequest);
    }


    /*[HttpGet("TestOptionPatterns")]
    public IActionResult Test()
    {
        return Ok(new { _jwtoptions.key, _jwtoptions.audience });
    }*/
}
