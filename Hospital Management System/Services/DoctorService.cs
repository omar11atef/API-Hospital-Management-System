using Hospital_Management_System.Entities;

namespace Hospital_Management_System.Services;

public class DoctorService (ApplicationDbContext context) : IDoctorService
{
    private readonly ApplicationDbContext _context = context;
    public async Task<Result<IEnumerable<Doctor>>> GetAllDoctorsAsync(CancellationToken cancellationToken =default)
    {
        var doctors = await _context.Doctors
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return Result.Success<IEnumerable<Doctor>>(doctors);
    }

    public async Task<Result<IEnumerable<Doctor>>> GetAllDoctorsExsitsAsync(CancellationToken cancellationToken = default)
    {
        var doctors = await _context.Doctors
            .AsNoTracking()
            .Where(d => d.IsDeleted)
            .ToListAsync(cancellationToken);
        return Result.Success<IEnumerable<Doctor>>(doctors);
    }

    public async Task<Result<ResponeDoctor>> GetDoctorByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await _context.Doctors.FindAsync(id, cancellationToken);
        return result is not null
            ? Result.Success(result.Adapt<ResponeDoctor>())
            : Result.Failure<ResponeDoctor>(DoctorErrors.DoctorNotFound);
    }

    public async Task<Result<ResponeDoctor>> CreateDoctorAsync(int departmentId,RequestDoctor doctor, CancellationToken cancellationToken = default)
    {
        var departmentExists = await _context.Departments
                .AnyAsync(d => d.Id == departmentId, cancellationToken);
        if (!departmentExists)
            return Result.Failure<ResponeDoctor>(DoctorErrors.DepartmentNotFound);

        var exists = await _context.Doctors
                            .AnyAsync(x => x.NationalId == doctor.NationalId, cancellationToken);
        if (exists) return Result.Failure<ResponeDoctor>(DoctorErrors.DuplicateNationalId);

        var newdoctor = doctor.Adapt<Doctor>();
        newdoctor.DepartmentId = departmentId;

        await _context.Doctors.AddAsync(newdoctor, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        var response = newdoctor.Adapt<ResponeDoctor>();
        return Result.Success(response);
    }

    public async Task<Result<ResponeDoctor>> UpdateDoctorAsync(int departmentId, int id, RequestDoctor doctor, CancellationToken cancellationToken = default)
    {
        var departmentExists = await _context.Departments
            .AnyAsync(d => d.Id == departmentId, cancellationToken);
        if (!departmentExists)
            return Result.Failure<ResponeDoctor>(DoctorErrors.DepartmentNotFound);

        var DoctorToUpdate = await _context.Doctors.FindAsync(id, cancellationToken);
        if (DoctorToUpdate is null)
            return Result.Failure<ResponeDoctor>(DoctorErrors.DoctorNotFound);

        bool isDuplicateNationalId = await _context.Doctors.AnyAsync(d =>
            d.NationalId == doctor.NationalId && d.Id != id, cancellationToken);
        if (isDuplicateNationalId)
            return Result.Failure<ResponeDoctor>(DoctorErrors.DuplicateNationalId);

        doctor.Adapt(DoctorToUpdate);
        DoctorToUpdate.Name = doctor.Name;
        DoctorToUpdate.Specialization = doctor.Specialization;
        DoctorToUpdate.Address = doctor.Address;
        DoctorToUpdate.DateOfBirth = doctor.DateOfBirth;
        DoctorToUpdate.AcademicDegree = doctor.AcademicDegree;
        DoctorToUpdate.PhoneNumber = doctor.PhoneNumber;
        DoctorToUpdate.NationalId = doctor.NationalId;

        DoctorToUpdate.DepartmentId = departmentId;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(DoctorToUpdate.Adapt<ResponeDoctor>());
    }

    public async Task<Result> DeleteDoctorAsync(int id, CancellationToken cancellationToken =default)
    {
        var doctor = await _context.Doctors.FindAsync(id, cancellationToken);
        if (doctor is null)
            return Result.Failure(DoctorErrors.DoctorNotFound);
        doctor.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> IsDoctorExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        var doctor = await _context.Doctors.FindAsync(id, cancellationToken);
        if(doctor is null)
            return Result.Failure(DoctorErrors.DoctorNotFound);
        doctor.IsDeleted = !doctor.IsDeleted;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<ResponseDoctorWithAppointments>> GetDoctorScheduleByIdAsync(int doctorId, CancellationToken cancellationToken)
    {
        var doctor = await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Department) 
            .Include(d => d.Appointments.Where(a => !a.IsDeleted)) 
            .ThenInclude(a => a.Patient)
            .FirstOrDefaultAsync(d => d.Id == doctorId, cancellationToken);

        if (doctor is null)
            return Result.Failure<ResponseDoctorWithAppointments>(DoctorErrors.DoctorNotFound);

        doctor.Appointments = doctor.Appointments.OrderBy(a => a.AppointmentDate).ToList();

        var response = doctor.Adapt<ResponseDoctorWithAppointments>();

        return Result.Success(response);
    }

   

}
