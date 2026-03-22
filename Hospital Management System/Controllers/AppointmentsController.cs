using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management_System.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AppointmentsController(IAppointmentService appointmentService) : ControllerBase
{
    private readonly IAppointmentService _appointmentService = appointmentService;


    // GET All appointment
    [HttpGet]
    public async Task<IActionResult> GetAllAppointments(CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentService.GetAllAppointmentsAsync(cancellationToken);
        var response = appointment.Value.Adapt<IEnumerable<AppointmentResponse>>();

        return appointment.IsSuccess
            ? Ok(response)
            : appointment.ToProblem(StatusCodes.Status400BadRequest);
    }

    // GET api/appointments/deleted
    [HttpGet("deleted")]
    public async Task<IActionResult> GetDeletedAppointments(CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService.GetDeletedAppointmentsAsync(cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

    // GET api/appointments/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAppointmentById([FromRoute] int id,CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService.GetAppointmentByIdAsync(id, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

    // POST api/doctors/{doctorId}/patients/{patientId}/appointments
    [HttpPost("{doctorId:int}/patients/{patientId:int}/appointments")]
    public async Task<IActionResult> CreateAppointment([FromRoute] int doctorId,[FromRoute] int patientId,[FromBody] AppointmentRequest request,CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService
            .CreateAppointmentAsync(doctorId, patientId, request, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetAppointmentById),new { id = result.Value.Id },result.Value);

        return result.Error.Equals(AppointmentErrors.DoctorNotFound) || result.Error.Equals(AppointmentErrors.PatientNotFound)
            ? result.ToProblem(StatusCodes.Status404NotFound)
            : result.ToProblem(StatusCodes.Status409Conflict);
    }

    // PUT api/appointments/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAppointment([FromRoute] int id,[FromBody] AppointmentRequest request,CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService.UpdateAppointmentAsync(id, request, cancellationToken);

        if (result.IsSuccess)
            return NoContent();

        return result.Error.Equals(AppointmentErrors.NotFound)
            ? result.ToProblem(StatusCodes.Status404NotFound)
            : result.ToProblem(StatusCodes.Status409Conflict);
    }

    // DELETE api/appointments/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAppointment([FromRoute] int id,CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService.DeleteAppointmentAsync(id, cancellationToken);

        if (result.IsSuccess)
            return NoContent();

        return result.Error.Equals(AppointmentErrors.NotFound)
            ? result.ToProblem(StatusCodes.Status404NotFound)
            : result.ToProblem(StatusCodes.Status409Conflict);
    }

    // POST api/appointments/{id}/toggle-status
    [HttpPost("{id:int}/toggle-status")]
    public async Task<IActionResult> ToggleAppointmentStatus([FromRoute] int id,CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService.ToggleAppointmentStatusAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

}
