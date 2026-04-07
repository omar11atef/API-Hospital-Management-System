namespace Hospital_Management_System.Controllers;

[Route("Account")]
[ApiController]
[Authorize]
public class AccountController(IUserService userService, IHttpContextAccessor httpContextAccessor , UserManager<ApplicationUser> userManager) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager=userManager;

    [HttpGet("info")]
    public async Task<IActionResult> Info()
    {

        var result = await _userService.GetProfileAsync(User.GetUserId()!);

        return Ok(result);
    }

    /*[HttpGet("my-identifiers")]
    public IActionResult GetAllNameIdentifiers()
    {
        // 1. Find ALL claims that match the NameIdentifier type
        var allNameIdentifiers = User.FindAll(ClaimTypes.NameIdentifier)
                                     .Select(claim => claim.Value)
                                     .ToList();
        // 2. Return them
            return Ok(new
            {
                TotalFound = allNameIdentifiers.Count,
                Values = allNameIdentifiers
           });
    }*/


    [HttpPut("info-Update")]
    public async Task<IActionResult> Info([FromBody] UpdateProfileRequest request)
    {
        await _userService.UpdateProfileAsync(User.GetUserId()!,request);
       
        return NoContent();
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequset request)
    {
        var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }


}


