using Hospital_Management_System.Authentication.Filter;
using Hospital_Management_System.Contracts.DepartmentRequest;
using Microsoft.AspNetCore.Components.Web;

namespace Hospital_Management_System.Controllers;

[Route("[controller]")]
[ApiController]
//[Authorize(Roles =DefaultRoles.Member)]
public class DepartmentController(IDepartmentService departmentService) : ControllerBase
{
    private readonly IDepartmentService _departmentService = departmentService;

    // GET All Departments
    [HttpGet("GETALL")]
    //[HasPermission(Permissions.GetAllDepartments)]
    public async Task<IActionResult> GetAllDepartments(CancellationToken cancellationToken = default)
    {
        var departmentsResult = await _departmentService.GetAllDepartmentsAsync(cancellationToken);
 
        return departmentsResult.IsSuccess? Ok(departmentsResult.Value.Adapt<IEnumerable<DepartmentResponse>>()): departmentsResult.ToProblem();
    }

    // GET By Id:
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDepartmentById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _departmentService.GetDepartmentByIdAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // POST: api/Departments
    [HttpPost]
    public async Task<IActionResult> CreateDepartment([FromBody] DepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _departmentService.CreateDepartmentAsync(request, cancellationToken);
  
        return result.IsSuccess
                ? CreatedAtAction(actionName: nameof(GetDepartmentById), routeValues: new { id = result.Value.Id }, value: result.Value)
                : result.ToProblem(); ;
    }

    // PUT: api/Departments/id
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDepartment([FromRoute] int id, [FromBody] DepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _departmentService.UpdateDepartmentAsync(id, request, cancellationToken);
 
        return result.IsSuccess
             ? NoContent()
             : result.ToProblem(); 
    }


}