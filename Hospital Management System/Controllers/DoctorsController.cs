using Hospital_Management_System.Entities;

namespace Hospital_Management_System.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class DoctorsController(IDoctorService doctorServices , IAppointmentService appointmentService) : ControllerBase
{

    private readonly IDoctorService _doctorServices = doctorServices;
    private readonly IAppointmentService _appointmentService = appointmentService;



    // GET All Doctors
    [HttpGet]
    public async Task<IActionResult> GetAllDoctors(CancellationToken cancellationToken = default)
    {
        var doctors = await _doctorServices.GetAllDoctorsAsync(cancellationToken);
        var response = doctors.Value.Adapt<IEnumerable<ResponeDoctor>>();

        return doctors.IsSuccess
            ? Ok(response)
            : doctors.ToProblem(StatusCodes.Status400BadRequest);
    }
    // GET All Doctors Exsits
    [HttpGet("Doctors Exsits")]
    public async Task<IActionResult> GetAllDoctorsExsits(CancellationToken cancellationToken = default)
    {
        var doctors = await _doctorServices.GetAllDoctorsExsitsAsync(cancellationToken);
        var response = doctors.Value.Adapt<IEnumerable<ResponeDoctor>>();
        return doctors.IsSuccess
           ? Ok(response)
           : doctors.ToProblem(StatusCodes.Status400BadRequest);
    }

    // GET Doctor by ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDoctorById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var DoctoerRsult = await _doctorServices.GetDoctorByIdAsync(id, cancellationToken);
        
        return DoctoerRsult.IsSuccess
            ? Ok(DoctoerRsult.Value)
            : DoctoerRsult.ToProblem(StatusCodes.Status404NotFound);
    }

 
    // POST Create a new Doctor
    [HttpPost("/api/departments/{departmentId:int}/[controller]")]
    public async Task<IActionResult> CreateDoctor([FromRoute]int departmentId,[FromBody] RequestDoctor doctor, CancellationToken cancellationToken = default)
    {
        var createdDoctor = await _doctorServices.CreateDoctorAsync(departmentId, doctor, cancellationToken);
        //var response = createdDoctor.Adapt<ResponeDoctor>();
        if (createdDoctor.IsSuccess)
            return CreatedAtAction(nameof(GetDoctorById), new { id = createdDoctor.Value.Id }, createdDoctor.Value);
        return createdDoctor.Error.Equals(DoctorErrors.DepartmentNotFound)
           ? createdDoctor.ToProblem(StatusCodes.Status404NotFound)
           : createdDoctor.ToProblem(StatusCodes.Status400BadRequest);
    }

    //Put Update Doctor
    [HttpPut("/api/departments/{departmentId:int}/[controller]/{id:int}")]
    public async Task<IActionResult> UpdateDoctor([FromRoute] int departmentId, [FromRoute] int id, [FromBody] RequestDoctor doctor, CancellationToken cancellationToken = default)
    {
        var updatedDoctor = await _doctorServices.UpdateDoctorAsync(departmentId, id, doctor, cancellationToken);
        if (updatedDoctor.IsSuccess)
            return NoContent();
        return updatedDoctor.Error.Equals(DoctorErrors.DuplicateNationalId)
            ? updatedDoctor.ToProblem(StatusCodes.Status409Conflict)
            : updatedDoctor.ToProblem(StatusCodes.Status404NotFound);
    }

    // DELETE Doctor
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDoctor([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _doctorServices.DeleteDoctorAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

    // Make Doctor Is Existed
    [HttpPut("{id}/ToggleExisted")]
    public async Task<IActionResult> IsDoctorExists([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _doctorServices.IsDoctorExistsAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

    // GET Doctor Appointments 
    [HttpGet("{id:int}/schedule")]
    public async Task<IActionResult> GetDoctorSchedule([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        //var result = await _doctorServices.GetDoctorScheduleByIdAsync(id, cancellationToken);
        var result = await _appointmentService.GetAppointmentsByDoctorAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

}
