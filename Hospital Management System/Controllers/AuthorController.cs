using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
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
    public async Task<IActionResult> IsAuthorCorrectAysun([FromBody] LoginRequest  authorRequest
        , CancellationTokenSource cancellationTokenSource)
    {
        var AuthorRuslt = await _authorService.IsAuthorCorrectAysun(authorRequest.Email, authorRequest.Password, cancellationTokenSource);
       
        return AuthorRuslt.IsSuccess
            ? Ok(AuthorRuslt.Value)
            : BadRequest(AuthorRuslt.Error);
            ;
    }

    [HttpGet("TestOptionPatterns")]
    public IActionResult Test()
    {
        return Ok(new { _jwtoptions.key, _jwtoptions.audience });
    }
}
