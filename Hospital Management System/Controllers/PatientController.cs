namespace Hospital_Management_System.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PatientController(IPatientsServices patients, IConfiguration configuration) : ControllerBase
{
    private readonly IPatientsServices _patients = patients;
    private readonly IConfiguration _configuration = configuration
      ?? throw new ArgumentNullException(nameof(configuration));
    // GET All Patients
    [HttpGet("GETALL")]
    public async Task<IActionResult> GetAllPatient(CancellationToken cancellationToken = default)
    {
        var patient = await _patients.GetAllPatientsAsync(cancellationToken);
        var respone = patient.Adapt<IEnumerable<ResponePatient>>();
        return Ok(respone);
    }

    // GET All Patients Deleted
    [HttpGet("GETALLDELETED")]
    public async Task<IActionResult> GetAllPatientDeleted(CancellationToken cancellationToken = default)
    {
        var patient = await _patients.GetAllPatientDeletedAsync(cancellationToken);
        var respone = patient.Adapt<IEnumerable<ResponePatient>>();
        return Ok(respone);
    }
    // GET All Patients Not Deleted
    [HttpGet("GETALLNOTDELETED")]
    public async Task<IActionResult> GetAllPatientNotDeleted(CancellationToken cancellationToken = default)
    {
        var patient = await _patients.GetAllPatientNotDeletedAsync(cancellationToken);
        var respone = patient.Adapt<IEnumerable<ResponePatient>>();
        return Ok(respone);
    }

    // GET Patient By Id
    [HttpGet("GETBYID/{id}")]
    public async Task<IActionResult> GetPatientById(int id, CancellationToken cancellationToken = default)
    {
        var patient = await _patients.GetPatientByIdAsync(id, cancellationToken);
        if (patient is null)
            return NotFound();
        var respone = patient.Adapt<ResponePatient>();
        return Ok(respone);
    }

    // Post Create New Patient
    [HttpPost("CreateNewPatient")]
    public async Task<IActionResult> CreateNewPatient([FromBody] RequestPatient request, CancellationToken cancellationToken = default)
    {
        if (_patients is null) return NotFound();
        var nationalIdExists = await _patients.GetAllPatientsAsync(cancellationToken);
        if (nationalIdExists.Any(p => p.NationalId == request.NationalId))
        {
            return BadRequest("That NationalId Already exists");
        }


        var createpatient = await _patients.CreatePatientAsync(request.Adapt<Patients>(), cancellationToken);
        if (createpatient is null)
            return BadRequest();
 
        return CreatedAtAction(nameof(GetPatientById), new { id = createpatient.Id }, createpatient.Adapt<ResponePatient>());
    }

    // Put Update Patient
    [HttpPut("UpdatePatient/{id}")]
    public async Task<IActionResult> UpdatePatient(int id, [FromBody] RequestPatient Updaterequest, CancellationToken cancellationToken = default)
    {
        if (_patients is null) return NotFound(); 
  
        var result = await _patients.UpdatePatientAsync(id, Updaterequest.Adapt<Patients>(), cancellationToken);
        if (!result)
            return NotFound();
        return NoContent();
    }

    // Delete Patient
    [HttpDelete("DeletePatient/{id}")]
    public async Task<IActionResult> DeletePatient(int id, CancellationToken cancellationToken = default)
    {
        if (_patients is null) return NotFound();

        var result = await _patients.DeletePatientAsync(id, cancellationToken);
        if (!result)
            return NotFound();
        return NoContent();
    }

    // Toggle Patient Exite
    [HttpPost("TogglePatientExite/{id}")]
   public async Task<IActionResult> TogglePatientExite(int id, CancellationToken cancellationToken = default)
    {
        if (_patients is null) return NotFound();
        var result = await _patients.TogglePatientExiteAsync(id, cancellationToken);
        if (!result)
            return NotFound();
        return NoContent();
    }

    // Update Max Medical Expenses
    /* [HttpPost("UpdateMaxMedicalExpenses/{id}")]
     public async Task<IActionResult> UpdateMaxMedicalExpenses(int id, [FromBody] decimal newMaxMedicalExpenses, CancellationToken cancellationToken = default)
     {
         if (_patients is null) return NotFound();

         var (IsSuccess, Message, NewAmount) = await _patients.UpdateMaxMedicalExpensesAsync(id, newMaxMedicalExpenses, cancellationToken);
         if (!IsSuccess)
             return NotFound();
         return NoContent();
     }*/

    [HttpPost("UpdateMaxMedicalExpenses/{id}")]
    public async Task<IActionResult> UpdateMaxMedicalExpenses(int id, [FromBody] UpdateExpensesRequest newMaxMedicalExpenses, CancellationToken cancellationToken = default)
    {
        if (_patients is null) return NotFound();

        var (IsSuccess, Message, NewAmount) = await _patients.UpdateMaxMedicalExpensesAsync(id, newMaxMedicalExpenses, cancellationToken);
        if (!IsSuccess)
            return NotFound();
        return NoContent();
    }
}
