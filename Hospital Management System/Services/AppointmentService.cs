namespace Hospital_Management_System.Services;

public class AppointmentService (ApplicationDbContext context) : IAppointmentService
{
    private readonly ApplicationDbContext _context = context;
    public async Task<Result<IEnumerable<Appointment>>> GetAllAppointmentsAsync(CancellationToken cancellationToken = default)
    {
        var appointments = await _context.Appointments
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return Result.Success<IEnumerable<Appointment>>(appointments);
    }
    public async Task<Result<AppointmentResponse>> GetAppointmentByIdAsync(int appointmentId,CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == appointmentId && !a.IsDeleted, cancellationToken);

        return appointment is not null
            ? Result.Success(appointment.Adapt<AppointmentResponse>())
            : Result.Failure<AppointmentResponse>(AppointmentErrors.NotFound);
    }
    public async Task<Result<IEnumerable<AppointmentResponse>>> GetDeletedAppointmentsAsync(CancellationToken cancellationToken = default)
    {
        var appointments = await _context.Appointments
            .AsNoTracking()
            .Where(a => a.IsDeleted)
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);

        if (!appointments.Any())
            return Result.Failure<IEnumerable<AppointmentResponse>>(
                AppointmentErrors.NoDeletedAppointmentsFound);

        return Result.Success(appointments.Adapt<IEnumerable<AppointmentResponse>>());
    }

    public async Task<Result<DoctorAppointmentsResponse>> GetAppointmentsByDoctorAsync(int doctorId,CancellationToken cancellationToken = default)
    {
        var doctor = await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Appointments.Where(a => !a.IsDeleted))
                .ThenInclude(a => a.Patient)
            .FirstOrDefaultAsync(d => d.Id == doctorId && !d.IsDeleted, cancellationToken);
        if (doctor is null)
            return Result.Failure<DoctorAppointmentsResponse>(AppointmentErrors.DoctorNotFound);

        doctor.Appointments = doctor.Appointments
            .OrderBy(a => a.AppointmentDate)
            .ToList();

        return Result.Success(doctor.Adapt<DoctorAppointmentsResponse>());
    }
    public async Task<Result<PatientAppointmentsResponse>> GetAppointmentsByPatientAsync(int patientId,CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
         .AsNoTracking()
         .Include(p => p.Department)                             
         .Include(p => p.Appointments.Where(a => !a.IsDeleted))
             .ThenInclude(a => a.Doctor)
         .FirstOrDefaultAsync(p => p.Id == patientId && !p.IsDeleted, cancellationToken);

        if (patient is null)
            return Result.Failure<PatientAppointmentsResponse>(AppointmentErrors.PatientNotFound);

        patient.Appointments = [.. patient.Appointments.OrderBy(a => a.AppointmentDate)];

        return Result.Success(patient.Adapt<PatientAppointmentsResponse>());
    }

    public async Task<Result<AppointmentResponse>> CreateAppointmentAsync(int doctorId,int patientId,AppointmentRequest request,CancellationToken cancellationToken = default)
    {
        var doctorExists = await _context.Doctors
            .AnyAsync(d => d.Id == doctorId && !d.IsDeleted, cancellationToken);
        if (!doctorExists)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.DoctorNotFound);

        var patientExists = await _context.Patients
            .AnyAsync(p => p.Id == patientId && !p.IsDeleted, cancellationToken);
        if (!patientExists)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.PatientNotFound);

        var hasConflict = await _context.Appointments
            .AnyAsync(a =>a.DoctorId == doctorId &&!a.IsDeleted ,cancellationToken);
        if (hasConflict)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.TimeConflict);

        var appointment = new Appointment
        {
            DoctorId = doctorId,
            PatientId = patientId,
            AppointmentDate = request.AppointmentDate,
            Status = request.Status,
            Notes = request.Notes,
            ReasonForVisit = request.ReasonForVisit
        };

        await _context.Appointments.AddAsync(appointment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Re-fetch with navigation properties for the response
        var result = await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .FirstAsync(a => a.Id == appointment.Id, cancellationToken);
        return Result.Success(result.Adapt<AppointmentResponse>());
    }
    public async Task<Result<AppointmentResponse>> UpdateAppointmentAsync(int appointmentId,AppointmentRequest request,CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == appointmentId && !a.IsDeleted, cancellationToken);
        if (appointment is null)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.NotFound);

        // Check conflict excluding the appointment being updated
        var hasConflict = await _context.Appointments
            .AnyAsync(a => a.Id != appointmentId && a.DoctorId == appointment.DoctorId && !a.IsDeleted ,cancellationToken);
        if (hasConflict)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.TimeConflict);

        appointment.AppointmentDate = request.AppointmentDate;
        appointment.Status = request.Status;
        appointment.Notes = request.Notes;
        appointment.ReasonForVisit = request.ReasonForVisit;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(appointment.Adapt<AppointmentResponse>());
    }

    public async Task<Result> DeleteAppointmentAsync(int appointmentId,CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .FindAsync(appointmentId, cancellationToken);

        if (appointment is null)
            return Result.Failure(AppointmentErrors.NotFound);

        /*if (appointment.IsDeleted)
            return Result.Failure(AppointmentErrors.AlreadyDeleted);*/

        appointment.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ToggleAppointmentStatusAsync(int id,CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (appointment is null)
            return Result.Failure(AppointmentErrors.NotFound);

        appointment.IsDeleted = !appointment.IsDeleted;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }


}

