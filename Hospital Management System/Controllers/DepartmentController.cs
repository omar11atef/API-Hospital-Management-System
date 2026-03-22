using Hospital_Management_System.Contracts.DepartmentRequest;

namespace Hospital_Management_System.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DepartmentController(IDepartmentService departmentService) : ControllerBase
{
    private readonly IDepartmentService _departmentService = departmentService;

    // GET All Departments
    [HttpGet("GETALL")]
    public async Task<IActionResult> GetAllDepartments(CancellationToken cancellationToken = default)
    {
        var departmentsResult = await _departmentService.GetAllDepartmentsAsync(cancellationToken);
        if (!departmentsResult.IsSuccess)
            return departmentsResult.ToProblem(StatusCodes.Status400BadRequest);
        return Ok(departmentsResult.Value.Adapt<IEnumerable<DepartmentResponse>>());
    }

    // GET By Id:
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDepartmentById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _departmentService.GetDepartmentByIdAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

    // POST: api/Departments
    [HttpPost]
    public async Task<IActionResult> CreateDepartment([FromBody] DepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _departmentService.CreateDepartmentAsync(request, cancellationToken);
        if (result.IsSuccess)
            return CreatedAtAction(actionName: nameof(GetDepartmentById), routeValues: new { id = result.Value.Id }, value: result.Value);

        return result.Error == DepartmentErrors.DuplicateName
                ? result.ToProblem(StatusCodes.Status409Conflict)
                : result.ToProblem(StatusCodes.Status400BadRequest); ;
    }

    // PUT: api/Departments/id
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDepartment([FromRoute] int id, [FromBody] DepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _departmentService.UpdateDepartmentAsync(id, request, cancellationToken);
        if (result.IsSuccess)
            return NoContent();

        return result.Error == DepartmentErrors.NotFound
             ? result.ToProblem(StatusCodes.Status404NotFound)
             : result.ToProblem(StatusCodes.Status409Conflict); 
    }


}