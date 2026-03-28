namespace Hospital_Management_System.Services;

public interface IPatientsServices
{
    Task<Result<IEnumerable<Patient>>> GetAllPatientsAsync(CancellationToken cancellationToken);
    Task<Result<IEnumerable<Patient>>> GetAllPatientDeletedAsync(CancellationToken cancellationToken);
    Task<Result<IEnumerable<Patient>>> GetAllPatientNotDeletedAsync(CancellationToken cancellationToken);
    Task<Result<ResponePatient>> GetPatientByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<ResponePatient>> CreatePatientAsync(int departmentId,RequestPatient patients, CancellationToken cancellationToken);
    Task<Result<ResponePatient>> UpdatePatientAsync(int departmentId,int id, RequestPatient patients, CancellationToken cancellationToken);
    Task<Result> DeletePatientAsync(int id, CancellationToken cancellationToken);
    Task<Result> TogglePatientExiteAsync(int id, CancellationToken cancellationToken);
    Task<Result<decimal>> UpdateMaxMedicalExpensesAsync(int id,UpdateExpensesRequest request,CancellationToken cancellationToken = default);

    Task<Result<PatientPdfReportData>> GetPatientReportDataAsync(int patientId, CancellationToken cancellationToken = default);

}
