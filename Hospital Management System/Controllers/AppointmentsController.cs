using QuestPDF.Fluent;

namespace Hospital_Management_System.Controllers;

[Route("[controller]")]
[ApiController]

public class AppointmentsController(IAppointmentService appointmentService) : ControllerBase
{
    private readonly IAppointmentService _appointmentService = appointmentService;


    // GET All appointment
    [HttpGet("GetAll")]
    
    public async Task<IActionResult> GetAllAppointments(CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService.GetAllAppointmentsAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // GET api/appointments/deleted
    [HttpGet("deleted")]
    public async Task<IActionResult> GetDeletedAppointments(CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService.GetDeletedAppointmentsAsync(cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // GET api/appointments/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAppointmentById([FromRoute] int id,CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService.GetAppointmentByIdAsync(id, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // POST api/doctors/{doctorId}/patients/{patientId}/appointments
    [HttpPost("{doctorId:int}/patients/{patientId:int}/appointments")]
    public async Task<IActionResult> CreateAppointment([FromRoute] int doctorId,[FromRoute] int patientId,[FromBody] AppointmentRequest request,CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService
            .CreateAppointmentAsync(doctorId, patientId, request, cancellationToken);

        return result.IsSuccess ? CreatedAtAction(nameof(GetAppointmentById), new { id = result.Value.Id }, result.Value) : result.ToProblem();
    }

    // PUT api/appointments/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAppointment([FromRoute] int id,[FromBody] AppointmentRequest request,CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService.UpdateAppointmentAsync(id, request, cancellationToken);
      
        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    // DELETE api/appointments/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAppointment([FromRoute] int id,CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService.DeleteAppointmentAsync(id, cancellationToken);

        return result.IsSuccess
           ? NoContent()
           : result.ToProblem();
    }

    // POST api/appointments/{id}/toggle-status
    [HttpPost("{id:int}/toggle-status")]
    public async Task<IActionResult> ToggleAppointmentStatus([FromRoute] int id,CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService.ToggleAppointmentStatusAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }


    [HttpPut("/rooms/appointments/{appointmentId:int}/cancel")]
    public async Task<IActionResult> CancelAppointment([FromRoute] int appointmentId, CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService.CancelAppointmentAsync(appointmentId, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem(); 
    }

    // GET: api/patients/{patientId}/appointments-history
    [HttpGet("{patientId:int}/appointments-history")]

    public async Task<IActionResult> GetPatientAppointmentHistory([FromRoute] int patientId,CancellationToken cancellationToken)
    {

        var result = await _appointmentService.GetPatientAppointmentHistoryAsync(patientId, cancellationToken);

        return result.IsSuccess? Ok(result.Value) : result.ToProblem();
    }

}
