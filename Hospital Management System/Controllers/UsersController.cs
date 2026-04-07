namespace Hospital_Management_System.Controllers;


[Route("[controller]")]
[ApiController]
[HasPermission(Permissions.GetUsers)]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("All-Users")]
    [HasPermission(Permissions.GetUsers)]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        return Ok( await _userService.GetAllAsync(cancellationToken));
    }

    [HttpGet("ById/{id:int}")]
    [HasPermission(Permissions.GetUsers)]
    public async Task<IActionResult> GetByIdUser([FromRoute] string id)
    {
        var result = await _userService.GetByIdUserAsync(id);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("Add-New-User")]
    [HasPermission(Permissions.AddUsers)]
    public async Task<IActionResult> AddNewUser([FromBody] CreateUserRequest request ,CancellationToken cancellationToken )
    {
        var result = await _userService.AddNewUserAsync(request , cancellationToken);
        return result.IsSuccess ? CreatedAtAction(nameof(AddNewUser), new { result.Value.Id},result.Value) : result.ToProblem();
    }

    [HttpPost("Update-User")]
    [HasPermission(Permissions.UpdateUsers)]
    public async Task<IActionResult> UpdateUsers([FromRoute] string id, UpdateProfileRequest request)
    {
        var result = await _userService.UpdateProfileAsync(id, request);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPost("Toggle-User")]
    public async Task<IActionResult> ToggleUser([FromRoute] string id)
    {
        var result = await _userService.ToggleStatuesAsync(id);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPost("Unlock-User")]
    public async Task<IActionResult> UnlockUser([FromRoute] string id)
    {
        var result = await _userService.UnlockAsync(id);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

}
