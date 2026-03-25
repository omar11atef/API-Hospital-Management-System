using Azure;
namespace Hospital_Management_System.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class PatientController(IPatientsServices patients, IConfiguration configuration , IAppointmentService appointmentService) : ControllerBase
{
    private readonly IPatientsServices _patients = patients;
    private readonly IConfiguration _configuration = configuration;
    private readonly IAppointmentService _appointmentService = appointmentService;

    // GET All Patients
    [HttpGet("GETALL")]
    public async Task<IActionResult> GetAllPatient(CancellationToken cancellationToken = default)
    {
        var patient = await _patients.GetAllPatientsAsync(cancellationToken);
        var respone = patient.Value.Adapt<IEnumerable<ResponePatient>>();
        return patient.IsSuccess
            ? Ok(respone)
            : patient.ToProblem(StatusCodes.Status400BadRequest);
    }

    // GET All Patients Deleted
    [HttpGet("GETALLDELETED")]
    public async Task<IActionResult> GetAllPatientDeleted(CancellationToken cancellationToken = default)
    {
        var patient = await _patients.GetAllPatientDeletedAsync(cancellationToken);
        var respone = patient.Value.Adapt<IEnumerable<ResponePatient>>();
        return patient.IsSuccess
            ? Ok(respone)
            : patient.ToProblem(StatusCodes.Status400BadRequest);
    }

    // GET All Patients Not Deleted
    [HttpGet("GETALLNOTDELETED")]
    public async Task<IActionResult> GetAllPatientNotDeleted(CancellationToken cancellationToken = default)
    {
        var patient = await _patients.GetAllPatientNotDeletedAsync(cancellationToken);
        var respone = patient.Value.Adapt<IEnumerable<ResponePatient>>();
        return patient.IsSuccess
            ? Ok(respone)
            : patient.ToProblem(StatusCodes.Status400BadRequest);
    }

    // GET Patient By Id
    [HttpGet("GETBYID/{id}")]
    public async Task<IActionResult> GetPatientById([FromRoute]int id, CancellationToken cancellationToken = default)
    {
        var patient = await _patients.GetPatientByIdAsync(id, cancellationToken);
        return patient.IsSuccess
            ? Ok(patient.Value)
            : patient.ToProblem(StatusCodes.Status404NotFound); 
    }

    //Post Create New Patient
    [HttpPost("/api/departments/{departmentId:int}/[controller]/AddNewPatient")]
    public async Task<IActionResult> CreateNewPatient([FromRoute] int departmentId, [FromBody] RequestPatient request, CancellationToken cancellationToken = default)
    {
        var createpatient = await _patients.CreatePatientAsync(departmentId, request, cancellationToken);
        if (createpatient.IsSuccess)
            return CreatedAtAction(nameof(GetPatientById), new { createpatient.Value.id }, createpatient.Value);

        return createpatient.Error.Equals(PatientErrors.DepartmentNotFound)
           ? createpatient.ToProblem(StatusCodes.Status404NotFound)
           : createpatient.ToProblem(StatusCodes.Status400BadRequest);
    }

    // Put Update Patient
    [HttpPut("/api/departments/{departmentId:int}/[controller]/UpdatePatient/{id:int}")]
    public async Task<IActionResult> UpdatePatient([FromRoute] int departmentId, [FromRoute] int id, [FromBody] RequestPatient Updaterequest, CancellationToken cancellationToken = default)
    {
        var result = await _patients.UpdatePatientAsync(departmentId, id, Updaterequest, cancellationToken);
        if (result.IsSuccess)
            return NoContent();

        return result.Error.Equals(PatientErrors.DuplicateNationalId)
            ? result.ToProblem(StatusCodes.Status409Conflict)
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

    // Delete Patient
    [HttpDelete("DeletePatient/{id}")]
    public async Task<IActionResult> DeletePatient(int id, CancellationToken cancellationToken = default)
    {
        var result = await _patients.DeletePatientAsync(id, cancellationToken);
        return result.IsSuccess
             ? NoContent()
             : result.ToProblem(StatusCodes.Status404NotFound);
    }

    // Toggle Patient Exite
    [HttpPost("TogglePatientExite/{id}")]
   public async Task<IActionResult> TogglePatientExite(int id, CancellationToken cancellationToken = default)
    {
        var result = await _patients.TogglePatientExiteAsync(id, cancellationToken);
        return result.IsSuccess
             ? NoContent()
             : result.ToProblem(StatusCodes.Status404NotFound);
    }

    // Update Max Medical Expenses
    [HttpPost("UpdateMaxMedicalExpenses/{id}")]
    public async Task<IActionResult> UpdateMaxMedicalExpenses(int id,[FromBody] UpdateExpensesRequest request,CancellationToken cancellationToken = default)
    {
        var result = await _patients
            .UpdateMaxMedicalExpensesAsync(id, request, cancellationToken);
        if (result.IsSuccess)
            return NoContent();

        return result.Error.Equals(PatientErrors.PatientNotFound)
            ? result.ToProblem(StatusCodes.Status404NotFound)
            : result.ToProblem(StatusCodes.Status400BadRequest);
    }

    // GET api/patients/{patientId}/appointments
    [HttpGet("{patientId:int}/appointments")]
    public async Task<IActionResult> GetPatientAppointments([FromRoute] int patientId,CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService.GetAppointmentsByPatientAsync(patientId, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

}
