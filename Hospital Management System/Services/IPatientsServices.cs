namespace Hospital_Management_System.Services;

public interface IPatientsServices
{
    Task<IEnumerable<Patients>> GetAllPatientsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Patients>> GetAllPatientDeletedAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Patients>> GetAllPatientNotDeletedAsync(CancellationToken cancellationToken);
    Task<Patients?> GetPatientByIdAsync(int id, CancellationToken cancellationToken);
    Task<Patients?> CreatePatientAsync(Patients patients, CancellationToken cancellationToken);
    Task<bool> UpdatePatientAsync(int id,Patients patients, CancellationToken cancellationToken);
    Task<bool> DeletePatientAsync(int id, CancellationToken cancellationToken);
    Task<bool> TogglePatientExiteAsync(int id, CancellationToken cancellationToken);
    Task<(bool IsSuccess, string Message, decimal? NewAmount)> UpdateMaxMedicalExpensesAsync(int id, UpdateExpensesRequest newMaxMedicalExpenses,
        CancellationToken cancellationToken = default);

}
