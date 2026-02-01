namespace Hospital_Management_System.Services;

public class DoctorServices : IDoctorServices
{
    private static readonly List<Doctor> _doctor = [
        new Doctor
        {
            Id = 1,
            Name = "Dr. John Smith",
            Specialization = "Cardiology",
            Address = "123 Main St, Cityville",
            DateOfBirth = new DateOnly(2022, 2, 1),
            AcademicDegree = "MD",
            TotalHoursWorked = 1500.5m,
            PhoneNumber = "555-1234",
            NationalId = "A123456789"
        },
        new Doctor
        {
            Id = 2,
            Name = "Dr. Emily Johnson",
            Specialization = "Neurology",
            Address = "456 Elm St, Townsville",
            DateOfBirth = new DateOnly(2000, 2, 1),
            AcademicDegree = "PhD",
            TotalHoursWorked = 1200.0m,
            PhoneNumber = "555-5678",
            NationalId = "B987654321"
        }
    ];

    public IEnumerable<Doctor> GetAllDoctors(CancellationToken cancellationToken =default)
    {
        return _doctor.Where(d => !d.IsDeleted);
    }

    public Doctor? GetDoctorById(int id, CancellationToken cancellationToken = default)
    {
        var doctor = _doctor.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
        if (doctor == null)
            return null;
        return doctor;
    }

    public Doctor? CreateDoctor(Doctor doctor, CancellationToken cancellationToken = default)
    {
        if (doctor is null)
            return null;

        // Auto-Increment ID Logic
        int newId = _doctor.Any() ? _doctor.Max(d => d.Id) + 1 : 1;
        doctor.Id = newId;

        // Set Defaults
        doctor.IsDeleted = false;
        doctor.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

        _doctor.Add(doctor);
        return doctor;
    }

    public Doctor? UpdateDoctor(int id, Doctor doctor, CancellationToken cancellationToken = default)
    {
        var DoctorToUpdate = GetDoctorById(id);
        if (DoctorToUpdate is null)
            return null;
        if (doctor is null)
            throw new ArgumentNullException(nameof(doctor), "Input doctor data cannot be null.");

        DoctorToUpdate.Name = doctor.Name;
        DoctorToUpdate.Specialization = doctor.Specialization;
        DoctorToUpdate.Address = doctor.Address;
        DoctorToUpdate.DateOfBirth = doctor.DateOfBirth;
        DoctorToUpdate.AcademicDegree = doctor.AcademicDegree;
        DoctorToUpdate.TotalHoursWorked = doctor.TotalHoursWorked;
        DoctorToUpdate.PhoneNumber = doctor.PhoneNumber;
        DoctorToUpdate.NationalId = doctor.NationalId;

        return DoctorToUpdate;
    }

    public bool DeleteDoctor(int id, CancellationToken cancellationToken =default)
    {
        var doctor = GetDoctorById(id);
        if (doctor is null)
            return false;
        _doctor.Remove(doctor);
        return true;
    }
}
