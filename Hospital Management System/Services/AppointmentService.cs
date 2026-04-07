namespace Hospital_Management_System.Services;

public class AppointmentService (ApplicationDbContext context) : IAppointmentService
{
    private readonly ApplicationDbContext _context = context;

    private static readonly HashSet<string> BlockingStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Booked", "Confirmed", "Waiting", "Postponed", "Rescheduled"
    };
    public async Task<Result<IEnumerable<AppointmentResponse>>> GetAllAppointmentsAsync(CancellationToken cancellationToken = default)
    {
        var appointments = await _context.Appointments
            .AsNoTracking()
            .Where(a => !a.IsDeleted)
            .Include(a => a.Doctor)   
            .Include(a => a.Patient)  
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
        var response = appointments.Select(MapToResponse);
        return Result.Success(response);
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
    public async Task<Result<PatientAppointmentsResponse>> GetAppointmentsByPatientAsync(int patientId, CancellationToken cancellationToken = default)
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
        var appointmentDate = request.AppointmentDate ?? DateTime.UtcNow;
        if (appointmentDate < DateTime.UtcNow.AddMinutes(-5))
            return Result.Failure<AppointmentResponse>(AppointmentErrors.CannotBookInPast);

        var slotStart = new DateTime(
        appointmentDate.Year,
        appointmentDate.Month,
        appointmentDate.Day,
        appointmentDate.Hour,
        appointmentDate.Minute, 
        0,
        DateTimeKind.Utc);

        var slotEnd = slotStart.AddHours(1);

        var doctorExists = await _context.Doctors
            .AnyAsync(d => d.Id == doctorId && !d.IsDeleted, cancellationToken);
        if (!doctorExists)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.DoctorNotFound);

        var patientExists = await _context.Patients
            .AnyAsync(p => p.Id == patientId && !p.IsDeleted, cancellationToken);
        if (!patientExists)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.PatientNotFound);

        var overlapLimitStart = slotStart.AddHours(-1); 

        var hasConflict = await _context.Appointments
            .AnyAsync(a =>
                a.DoctorId == doctorId &&
                !a.IsDeleted &&
                a.AppointmentDate > overlapLimitStart && 
                a.AppointmentDate < slotEnd,
            cancellationToken);

        if (hasConflict)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.TimeConflict);

        var appointment = new Appointment
        {
            DoctorId = doctorId,
            PatientId = patientId,
            AppointmentDate = slotStart,  
            Status = request.Status ?? "Scheduled",
            Notes = request.Notes,
            ReasonForVisit = request.ReasonForVisit
        };

        await _context.Appointments.AddAsync(appointment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

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

        var newDate = request.AppointmentDate ?? appointment.AppointmentDate;
        var (slotStart, slotEnd) = GetSlotBoundaries(newDate);

        var hasConflict = await _context.Appointments
            .AnyAsync(a =>
                a.Id != appointmentId &&   
                a.DoctorId == appointment.DoctorId &&
                !a.IsDeleted &&
                a.AppointmentDate >= slotStart &&
                a.AppointmentDate < slotEnd,
            cancellationToken);

        if (hasConflict)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.TimeConflict);

        appointment.AppointmentDate = newDate;
        appointment.Status = request.Status;
        appointment.Notes = request.Notes;
        appointment.ReasonForVisit = request.ReasonForVisit;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(appointment));
    }
    public async Task<Result> DeleteAppointmentAsync(int appointmentId,CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .FindAsync(appointmentId, cancellationToken);

        if (appointment is null)
            return Result.Failure(AppointmentErrors.NotFound);

        /*if (appointment.IsDeleted)
            return Result.Failure(AppointmentErrors.AlreadyDeleted);*/
        if (appointment.IsDeleted)
            return Result.Success();
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
    public async Task<Result<PatientHistoryResponse>> GetPatientAppointmentHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == patientId && !p.IsDeleted, cancellationToken);

        if (patient is null)
            return Result.Failure<PatientHistoryResponse>(PatientErrors.PatientNotFound);

        var patientName = patient.Name;

        var rawAppointments = await _context.Appointments
            .IgnoreQueryFilters() 
            .AsNoTracking()
            .Where(a => a.PatientId == patientId) 
            .Select(a => new
            {
                a.Id,
                DoctorName = a.Doctor.Name,
                PatientName = patientName,
                a.AppointmentDate,
                a.Status,
                a.Notes,
                a.ReasonForVisit,
                a.IsDeleted
            })
            .ToListAsync(cancellationToken);


        var now = DateTime.UtcNow;

        var upcoming = rawAppointments
            .Where(a => a.AppointmentDate >= now)
            .OrderBy(a => a.AppointmentDate)
            .Select(a => new AppointmentResponse(
                a.Id,
                a.DoctorName,
                a.PatientName,
                a.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
                BuildSlotDisplay(a.AppointmentDate),
                a.Status,
                a.Notes,
                a.ReasonForVisit,
                a.IsDeleted
            )).ToList();

        var past = rawAppointments
            .Where(a => a.AppointmentDate < now)
            .OrderByDescending(a => a.AppointmentDate)
            .Select(a => new AppointmentResponse(
                a.Id,
                a.DoctorName,
                a.PatientName,
                a.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
                BuildSlotDisplay(a.AppointmentDate),
                a.Status,
                a.Notes,
                a.ReasonForVisit,
                a.IsDeleted
            )).ToList();

        var response = new PatientHistoryResponse(PatientId: patientId,PatientName: patientName,UpcomingAppointments: upcoming,PastAppointments: past);

        return Result.Success(response);
    }

    // Cancle Aappoitment
    public async Task<Result<CancelAppointmentResponse>> CancelAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId && !a.IsDeleted, cancellationToken);

        if (appointment is null)
            return Result.Failure<CancelAppointmentResponse>(RoomErrors.AppointmentNotFound);

        if (appointment.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<CancelAppointmentResponse>(RoomErrors.AppointmentAlreadyCancelled);

        if (appointment.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<CancelAppointmentResponse>(RoomErrors.AppointmentAlreadyCompleted);

        var previousStatus = appointment.Status;

        var patientRoom = await _context.PatientRooms
            .Include(pr => pr.Room)
            .FirstOrDefaultAsync(pr => pr.AppointmentId == appointmentId && !pr.IsDeleted, cancellationToken);

        string? freedRoomNumber = null;

        if (patientRoom is not null)
        {
            patientRoom.IsDeleted = true;

            var roomStillOccupied = await _context.PatientRooms
                .Where(pr => pr.RoomId == patientRoom.RoomId && pr.Id != patientRoom.Id && !pr.IsDeleted)
                .Include(pr => pr.Appointment)
                .AnyAsync(pr => BlockingStatuses.Contains(pr.Appointment.Status), cancellationToken);

            patientRoom.Room.IsOccupied = roomStillOccupied;
            freedRoomNumber = patientRoom.Room.RoomNumber;
        }

        appointment.Status = "Cancelled";
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new CancelAppointmentResponse(
            AppointmentId: appointment.Id,
            PreviousStatus: previousStatus,
            NewStatus: "Cancelled",
            AppointmentDate: appointment.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
            SlotDisplay: BuildSlotDisplay(appointment.AppointmentDate),
            RoomFreed: patientRoom is not null,
            FreedRoomNumber: freedRoomNumber
        ));
    }

    // Private Methods :
    private static (DateTime slotStart, DateTime slotEnd) GetSlotBoundaries(DateTime dt)
    {
        var slotStart = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, DateTimeKind.Utc);
        return (slotStart, slotStart.AddHours(1));
    }
    private static AppointmentResponse MapToResponse(Appointment a) => new(
       Id: a.Id,
       DoctorName: a.Doctor?.Name ?? "Unknown",
       PatientName: a.Patient?.Name ?? "Unknown",
       AppointmentDate: a.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
       SlotDisplay: $"{a.AppointmentDate.Hour:D2}:00 – {a.AppointmentDate.Hour + 1:D2}:00",
       Status: a.Status,
       Notes: a.Notes,
       ReasonForVisit: a.ReasonForVisit,
       IsDeleted: a.IsDeleted
   );
    private static string BuildSlotDisplay(DateTime dt)
        => $"{dt.Hour:D2}:00 – {dt.Hour + 1:D2}:00";


}

