using Azure;
using Hospital_Management_System.Services;
using QuestPDF.Fluent;
namespace Hospital_Management_System.Controllers;

[Route("[controller]")]
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
            : patient.ToProblem();
    }

    // GET All Patients Deleted
    [HttpGet("GETALLDELETED")]
    public async Task<IActionResult> GetAllPatientDeleted(CancellationToken cancellationToken = default)
    {
        var patient = await _patients.GetAllPatientDeletedAsync(cancellationToken);
        var respone = patient.Value.Adapt<IEnumerable<ResponePatient>>();
        return patient.IsSuccess
            ? Ok(respone)
            : patient.ToProblem();
    }

    // GET All Patients Not Deleted
    [HttpGet("GETALLNOTDELETED")]
    public async Task<IActionResult> GetAllPatientNotDeleted(CancellationToken cancellationToken = default)
    {
        var patient = await _patients.GetAllPatientNotDeletedAsync(cancellationToken);
        var respone = patient.Value.Adapt<IEnumerable<ResponePatient>>();
        return patient.IsSuccess
            ? Ok(respone)
            : patient.ToProblem();
    }

    // GET Patient By Id
    [HttpGet("GETBYID/{id}")]
    public async Task<IActionResult> GetPatientById([FromRoute]int id, CancellationToken cancellationToken = default)
    {
        var patient = await _patients.GetPatientByIdAsync(id, cancellationToken);
        return patient.IsSuccess
            ? Ok(patient.Value)
            : patient.ToProblem(); 
    }

    //Post Create New Patient
    [HttpPost("/departments/{departmentId:int}/[controller]/AddNewPatient")]
    public async Task<IActionResult> CreateNewPatient([FromRoute] int departmentId, [FromBody] RequestPatient request, CancellationToken cancellationToken = default)
    {
        var createpatient = await _patients.CreatePatientAsync(departmentId, request, cancellationToken);

        return createpatient.IsSuccess
           ? CreatedAtAction(nameof(GetPatientById), new { createpatient.Value.id }, createpatient.Value)
           : createpatient.ToProblem();
    }

    // Put Update Patient
    [HttpPut("/departments/{departmentId:int}/[controller]/UpdatePatient/{id:int}")]
    public async Task<IActionResult> UpdatePatient([FromRoute] int departmentId, [FromRoute] int id, [FromBody] RequestPatient Updaterequest, CancellationToken cancellationToken = default)
    {
        var result = await _patients.UpdatePatientAsync(departmentId, id, Updaterequest, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    // Delete Patient
    [HttpDelete("DeletePatient/{id}")]
    public async Task<IActionResult> DeletePatient(int id, CancellationToken cancellationToken = default)
    {
        var result = await _patients.DeletePatientAsync(id, cancellationToken);
        return result.IsSuccess
             ? NoContent()
             : result.ToProblem();
    }

    // Toggle Patient Exite
    [HttpPost("TogglePatientExite/{id}")]
   public async Task<IActionResult> TogglePatientExite(int id, CancellationToken cancellationToken = default)
    {
        var result = await _patients.TogglePatientExiteAsync(id, cancellationToken);
        return result.IsSuccess
             ? NoContent()
             : result.ToProblem();
    }

    // Update Max Medical Expenses
    [HttpPost("UpdateMaxMedicalExpenses/{id}")]
    public async Task<IActionResult> UpdateMaxMedicalExpenses(int id,[FromBody] UpdateExpensesRequest request,CancellationToken cancellationToken = default)
    {
        var result = await _patients
            .UpdateMaxMedicalExpensesAsync(id, request, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    // GET api/patients/{patientId}/appointments
    [HttpGet("{patientId:int}/appointments")]
    public async Task<IActionResult> GetPatientAppointments([FromRoute] int patientId,CancellationToken cancellationToken = default)
    {
        var result = await _appointmentService.GetAppointmentsByPatientAsync(patientId, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }
    // GET download-report for Appointment patients:
    [HttpGet("{patientId:int}/download-report")]
    public async Task<IActionResult> DownloadPatientReport([FromRoute] int patientId,CancellationToken cancellationToken = default)
    {
        var result = await _patients.GetPatientReportDataAsync(patientId, cancellationToken);

        if (result.IsSuccess)
        
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
       
        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "logo.png");
        byte[]? logoBytes = null;

        if (System.IO.File.Exists(logoPath))
        {
            logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath, cancellationToken);
        }

        var document = new PatientReportDocument(result.Value, logoBytes!);
        byte[] pdfBytes = document.GeneratePdf();
        string safePatientName = result.Value.PatientName.Replace(" ", "_");
        string fileName = $"MedicalReport_{safePatientName}_{DateTime.Now:yyyyMMdd}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }

}
