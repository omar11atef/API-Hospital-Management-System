namespace Hospital_Management_System.Services;

public interface IDoctorService
{
    Task<Result<IEnumerable<Doctor>>> GetAllDoctorsAsync(CancellationToken cancellationToken);
    Task<Result<IEnumerable<Doctor>>> GetAllDoctorsExsitsAsync(CancellationToken cancellationToken);
    Task<Result<ResponeDoctor>> GetDoctorByIdAsync(int id, CancellationToken cancellationToken);
    //Task<Result<ResponeDoctor>> CreateDoctorAsync( RequestDoctor doctor, CancellationToken cancellationToken);
    Task<Result<ResponeDoctor>> CreateDoctorAsync(int departmentId, RequestDoctor doctor, CancellationToken cancellationToken);
    Task<Result<ResponeDoctor>> UpdateDoctorAsync(int departmentId, int id, RequestDoctor doctor, CancellationToken cancellationToken);
    Task<Result> DeleteDoctorAsync(int id, CancellationToken cancellationToken);
    Task<Result> IsDoctorExistsAsync(int id, CancellationToken cancellationToken);

    Task<Result<ResponseDoctorWithAppointments>> GetDoctorScheduleByIdAsync(int doctorId, CancellationToken cancellationToken);
}
