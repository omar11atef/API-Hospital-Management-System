/*namespace Hospital_Management_System.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomsController(IRoomService roomService) : ControllerBase
{
    private readonly IRoomService _roomService = roomService;

    // GET /api/rooms
    [HttpGet]
    public async Task<IActionResult> GetAllRooms(CancellationToken cancellationToken = default)
    {
        var result = await _roomService.GetAllRoomsAsync(cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem(StatusCodes.Status400BadRequest);
    }

    // GET /api/rooms/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRoomById([FromRoute] int id,CancellationToken cancellationToken = default)
    {
        var result = await _roomService.GetRoomByIdAsync(id, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

    // POST /api/departments/{departmentId}/rooms
    [HttpPost("/api/departments/{departmentId:int}/rooms")]
    public async Task<IActionResult> CreateRoom([FromRoute] int departmentId,[FromBody] RoomRequest request,CancellationToken cancellationToken = default)
    {
        var result = await _roomService
            .CreateRoomAsync(departmentId, request, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetRoomById), new { id = result.Value.Id }, result.Value);

        return result.Error.Equals(RoomErrors.DepartmentNotFound)
            ? result.ToProblem(StatusCodes.Status404NotFound)
            : result.ToProblem(StatusCodes.Status409Conflict);
    }
    //! Note --< return ok() Wrong :
    // ASSIGN ROOM TO APPOINTMENT
    // POST /api/rooms/{roomId}/patients/{patientId}/appointments/{appointmentId}
    [HttpPost("{roomId:int}/patients/{patientId:int}/appointments/{appointmentId:int}")]
    public async Task<IActionResult> AssignRoomToAppointment([FromRoute] int roomId,[FromRoute] int patientId,[FromRoute] int appointmentId,[FromBody] AssignRoomRequest request,CancellationToken cancellationToken = default)
    {
        var result = await _roomService
            .AssignRoomToAppointmentAsync(roomId, patientId, appointmentId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            
            var is404 = result.Error.Equals(RoomErrors.NotFound) ||result.Error.Equals(RoomErrors.PatientNotFound) ||result.Error.Equals(RoomErrors.AppointmentNotFound);

            return is404
                ? result.ToProblem(StatusCodes.Status404NotFound)
                : result.ToProblem(StatusCodes.Status409Conflict);
        }

        return Ok(result.Value);   // 200 with the full assignment response
    }

    // GET DOCTOR APPOINTMENT ROOM
    // GET /api/rooms/doctors/{doctorId}
    [HttpGet("doctors/{doctorId:int}")]
    public async Task<IActionResult> GetDoctorAppointmentRoom([FromRoute] int doctorId,CancellationToken cancellationToken = default)
    {
        var result = await _roomService
            .GetDoctorAppointmentRoomAsync(doctorId, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

    // GET PATIENT APPOINTMENT ROOM
    // GET /api/rooms/patients/{patientId}
    [HttpGet("patients/{patientId:int}")]
    public async Task<IActionResult> GetPatientAppointmentRoom([FromRoute] int patientId,CancellationToken cancellationToken = default)
    {
        var result = await _roomService
            .GetPatientAppointmentRoomAsync(patientId, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem(StatusCodes.Status404NotFound);
    }
}*/

namespace Hospital_Management_System.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController(IRoomService roomService) : ControllerBase
{
    private readonly IRoomService _roomService = roomService;

    [HttpGet]
    public async Task<IActionResult> GetAllRooms(CancellationToken cancellationToken = default)
    {
        var result = await _roomService.GetAllRoomsAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem(StatusCodes.Status400BadRequest);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRoomById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _roomService.GetRoomByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpGet("{roomId:int}/appointments")]
    public async Task<IActionResult> GetRoomAppointments([FromRoute] int roomId, CancellationToken cancellationToken = default)
    {
        var result = await _roomService.GetRoomAppointmentsAsync(roomId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpPost("/api/departments/{departmentId:int}/rooms")]
    public async Task<IActionResult> CreateRoom([FromRoute] int departmentId, [FromBody] RoomRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _roomService.CreateRoomAsync(departmentId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error.Equals(RoomErrors.DepartmentNotFound)
                ? result.ToProblem(StatusCodes.Status404NotFound)
                : result.ToProblem(StatusCodes.Status409Conflict);
        }

        return CreatedAtAction(nameof(GetRoomById), new { id = result.Value.Id }, result.Value);
    }

    
    [HttpPost("{roomId:int}/patients/{patientId:int}/appointments/{appointmentId:int}")]
    public async Task<IActionResult> AssignRoomToAppointment([FromRoute] int roomId, [FromRoute] int patientId, [FromRoute] int appointmentId, [FromBody] AssignRoomRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _roomService.AssignRoomToAppointmentAsync(roomId, patientId, appointmentId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            var is404 = result.Error.Equals(RoomErrors.NotFound) ||
                        result.Error.Equals(RoomErrors.PatientNotFound) ||
                        result.Error.Equals(RoomErrors.AppointmentNotFound);

            return is404
                ? result.ToProblem(StatusCodes.Status404NotFound)
                : result.ToProblem(StatusCodes.Status409Conflict);
        }

        return Ok(result.Value);
    }


    [HttpGet("doctors/{doctorId:int}")]
    public async Task<IActionResult> GetDoctorAppointmentRoom([FromRoute] int doctorId, CancellationToken cancellationToken = default)
    {
        var result = await _roomService.GetDoctorAppointmentRoomAsync(doctorId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem(StatusCodes.Status404NotFound);
    }

    
    [HttpGet("patients/{patientId:int}")]
    public async Task<IActionResult> GetPatientAppointmentRoom([FromRoute] int patientId, CancellationToken cancellationToken = default)
    {
        var result = await _roomService.GetPatientAppointmentRoomAsync(patientId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem(StatusCodes.Status404NotFound);
    }

    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRoom([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _roomService.DeleteRoomAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem(StatusCodes.Status404NotFound);
    }

    
    [HttpPut("{id:int}/toggle")]
    public async Task<IActionResult> ToggleRoom([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _roomService.ToggleRoomStatusAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem(StatusCodes.Status404NotFound);
    }
}
