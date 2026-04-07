using Hospital_Management_System.Contracts.Roles;

namespace Hospital_Management_System.Controllers;

[Route("[controller]")]
[ApiController]
public class RolesController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService =roleService;

    [HttpGet("")]
    //[HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDisable,CancellationToken cancellationToken)
    {
        var roles = await _roleService.GetAllAsync(includeDisable, cancellationToken);
        return Ok(roles);
    }


    [HttpGet("GetById/{id}")]
    //[HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var roles = await _roleService.GetBuIdAsync(id);
        return roles.IsSuccess ? Ok(roles) : roles.ToProblem();
    }


    [HttpPost("AddRole")]
    //[HasPermission(Permissions.AddRole)]
    public async Task<IActionResult> AddRole([FromBody] RoleRequest request)
    {
        var roles = await _roleService.AddRoleAsync(request);

        return roles.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { roles.Value.Id }, roles.Value)
            : roles.ToProblem();
    }


    [HttpPut("{id}")]
    //[HasPermission(Permissions.UpdateRole)]
    public async Task<IActionResult> UpdateRole([FromRoute] string id, [FromBody] RoleRequest request)
    {
        var roles = await _roleService.UpdateRoleAsync(id, request);

        return roles.IsSuccess
            ? NoContent()
            : roles.ToProblem();
    }

    [HttpPut("Toggle/{id}")]
    //[HasPermission(Permissions.UpdateRole)]
    public async Task<IActionResult> ToggleRole([FromRoute] string id)
    {
        var roles = await _roleService.ToogleRoleAsync(id);

        return roles.IsSuccess
            ? NoContent()
            : roles.ToProblem();
    }
}
