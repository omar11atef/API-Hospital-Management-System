namespace Hospital_Management_System.Services;

public class DoctorServices (ApplicationDbContext context) : IDoctorServices
{
    private readonly ApplicationDbContext _context = context;
    /*private static readonly List<Doctor> _context = [
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
    ];*/

    public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync(CancellationToken cancellationToken =default)
    {
        return await _context.Doctors
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Doctor>> GetAllDoctorsExsitsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .AsNoTracking()
            .Where(d => d.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task <Doctor?> GetDoctorByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors.FindAsync(id, cancellationToken);
    }
    
    public async Task<Doctor?> CreateDoctorAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        if (doctor is null)
            return null!;
        await _context.AddAsync(doctor, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return doctor;
    }
    
    public async Task<Doctor?> UpdateDoctorAsync(int id, Doctor doctor, CancellationToken cancellationToken = default)
    {
        var DoctorToUpdate = await GetDoctorByIdAsync(id);
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
        
        await _context.SaveChangesAsync(cancellationToken);

        return DoctorToUpdate;
    }
    
    public async Task<bool> DeleteDoctorAsync(int id, CancellationToken cancellationToken =default)
    {
        var doctor = await GetDoctorByIdAsync(id);
        if (doctor is null)
            return false;
        _context.Remove(doctor);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> IsDoctorExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        var doctor = await GetDoctorByIdAsync(id);
        if(doctor is null)
            return false;
        doctor.IsDeleted = !doctor.IsDeleted;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

}
