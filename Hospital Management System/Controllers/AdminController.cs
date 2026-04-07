namespace Hospital_Management_System.Controllers;

[Route("[controller]")]
[ApiController]
public class AdminController(IAdminDashboardService adminDashboardService , IDepartmentService departmentService) : ControllerBase
{
    private readonly IAdminDashboardService _adminDashboardService= adminDashboardService;
    private readonly IDepartmentService _departmentService = departmentService;

    //private readonly IValidator<DashboardSummaryResponse> _validator=validator;

    // GET: api/departments/{departmentId}/daily-summary
    [HttpGet("{departmentId:int}/daily-summary")]
    public async Task<IActionResult> GetDailyDepartmentSummary([FromRoute] int departmentId,CancellationToken cancellationToken)
    {
        var result = await _adminDashboardService.GetDailyDepartmentSummaryAsync(departmentId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // GET: api/Admin/dashboard/occupancy
    [HttpGet("dashboard/Room/occupancy")]
    public async Task<IActionResult> GetRoomOccupancy(CancellationToken cancellationToken)
    {
        var result = await _adminDashboardService.GetRoomOccupancyRateAsync(cancellationToken);
        
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // GET: api/Admin/dashboard/appointments/today
    [HttpGet("dashboard/appointments/today")]
    public async Task<IActionResult> GetTodayAppointmentsSummary(CancellationToken cancellationToken)
    {
        var result = await _adminDashboardService.GetTodayAppointmentsSummaryAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // GET: api/Admin/dashboard/top-doctors?take=5
    [HttpGet("dashboard/top-doctors")]
    public async Task<IActionResult> GetTopDoctors([FromQuery] TopDoctorsRequest request,CancellationToken cancellationToken)
    {
        var result = await _adminDashboardService.GetTopDoctorsAsync(request.Take, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


}
