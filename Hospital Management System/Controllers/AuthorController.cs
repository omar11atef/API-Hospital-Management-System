using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management_System.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthorController(IAuthorService authorService) : ControllerBase
{
    private readonly IAuthorService _authorService = authorService;

    [HttpPost("")]
    public async Task<IActionResult> IsAuthorCorrectAysun([FromBody] AuthorRequest authorRequest
        , CancellationTokenSource cancellationTokenSource)
    {
        var authorResponse = await _authorService.IsAuthorCorrectAysun(authorRequest.Email, authorRequest.Password, cancellationTokenSource);
        if (authorResponse == null)
            return Unauthorized("Invalid Email Or Password");
        return Ok(authorResponse);
    }
}
