using Hospital_Management_System.Authentication.Filter;
using Hospital_Management_System.Entities;

namespace Hospital_Management_System.Controllers;

[Route("[controller]")]
[ApiController]

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
            : doctors.ToProblem();
    }
    // GET All Doctors Exsits
    [HttpGet("Doctors Exsits")]
    [HasPermission(Permissions.GetExistsDoctors)]
    public async Task<IActionResult> GetAllDoctorsExsits(CancellationToken cancellationToken = default)
    {
        var doctors = await _doctorServices.GetAllDoctorsExsitsAsync(cancellationToken);
        var response = doctors.Value.Adapt<IEnumerable<ResponeDoctor>>();
        return doctors.IsSuccess
           ? Ok(response)
           : doctors.ToProblem();
    }

    // GET Doctor by ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDoctorById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var DoctoerRsult = await _doctorServices.GetDoctorByIdAsync(id, cancellationToken);
        
        return DoctoerRsult.IsSuccess
            ? Ok(DoctoerRsult.Value)
            : DoctoerRsult.ToProblem();
    }

 
    // POST Create a new Doctor
    [HttpPost("/departments/{departmentId:int}/[controller]")]
    public async Task<IActionResult> CreateDoctor([FromRoute]int departmentId,[FromBody] RequestDoctor doctor, CancellationToken cancellationToken = default)
    {
        var createdDoctor = await _doctorServices.CreateDoctorAsync(departmentId, doctor, cancellationToken);
        //var response = createdDoctor.Adapt<ResponeDoctor>();

        return createdDoctor.IsSuccess
           ? CreatedAtAction(nameof(GetDoctorById), new { id = createdDoctor.Value.Id }, createdDoctor.Value)
           : createdDoctor.ToProblem();
    }

    //Put Update Doctor
    [HttpPut("/departments/{departmentId:int}/[controller]/{id:int}")]
    public async Task<IActionResult> UpdateDoctor([FromRoute] int departmentId, [FromRoute] int id, [FromBody] RequestDoctor doctor, CancellationToken cancellationToken = default)
    {
        var updatedDoctor = await _doctorServices.UpdateDoctorAsync(departmentId, id, doctor, cancellationToken);

        return updatedDoctor.IsSuccess
            ? NoContent()
            : updatedDoctor.ToProblem();
    }

    // DELETE Doctor
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDoctor([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _doctorServices.DeleteDoctorAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    // Make Doctor Is Existed
    [HttpPut("{id}/ToggleExisted")]
    public async Task<IActionResult> IsDoctorExists([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _doctorServices.IsDoctorExistsAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    // GET Doctor Appointments 
    [HttpGet("{id:int}/schedule")]
    public async Task<IActionResult> GetDoctorSchedule([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        //var result = await _doctorServices.GetDoctorScheduleByIdAsync(id, cancellationToken);
        var result = await _appointmentService.GetAppointmentsByDoctorAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

}
