namespace Hospital_Management_System.Services;

public interface IDoctorServices
{
    IEnumerable<Doctor> GetAllDoctors(CancellationToken cancellationToken);
    Doctor? GetDoctorById(int id, CancellationToken cancellationToken);
    Doctor? CreateDoctor(Doctor doctor, CancellationToken cancellationToken);
    Doctor? UpdateDoctor(int id, Doctor doctor, CancellationToken cancellationToken);
    bool DeleteDoctor(int id, CancellationToken cancellationToken);
}
