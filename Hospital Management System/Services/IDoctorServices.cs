namespace Hospital_Management_System.Services;

public interface IDoctorServices
{
    Task<IEnumerable<Doctor>> GetAllDoctorsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Doctor>> GetAllDoctorsExsitsAsync(CancellationToken cancellationToken);
    Task<Doctor?> GetDoctorByIdAsync(int id, CancellationToken cancellationToken);
    Task<Doctor?> CreateDoctorAsync(Doctor doctor, CancellationToken cancellationToken);
    Task<Doctor?> UpdateDoctorAsync(int id, Doctor doctor, CancellationToken cancellationToken);
    Task<bool> DeleteDoctorAsync(int id, CancellationToken cancellationToken);
    Task<bool> IsDoctorExistsAsync(int id, CancellationToken cancellationToken);
}
