using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Hospital_Management_System.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthorController(IAuthorService authorService, IOptions<JwtOptions> jwtoptions, IConfiguration configuration) : ControllerBase
{
    private readonly IAuthorService _authorService = authorService;
    private readonly JwtOptions _jwtoptions=jwtoptions.Value;
    private readonly IConfiguration _configuration = configuration;
    [HttpPost("")]
    public async Task<IActionResult> IsAuthorCorrectAysun([FromBody] AuthorRequest authorRequest
        , CancellationTokenSource cancellationTokenSource)
    {
        var authorResponse = await _authorService.IsAuthorCorrectAysun(authorRequest.Email, authorRequest.Password, cancellationTokenSource);
        if (authorResponse == null)
            return Unauthorized("Invalid Email Or Password");
        return Ok(authorResponse);
    }

    [HttpGet("TestOptionPatterns")]
    public IActionResult Test()
    {
        return Ok(new { _jwtoptions.key, _jwtoptions.audience });
    }
}
