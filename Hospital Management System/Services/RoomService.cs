/*using Hospital_Management_System.Contracts.Room;
using Microsoft.Extensions.Caching.Hybrid;

namespace Hospital_Management_System.Services;

public class RoomService (ApplicationDbContext context , HybridCache hybridCache , ILogger logger) : IRoomService
{
    private readonly ApplicationDbContext _context = context;
    private readonly HybridCache _hybridCache= hybridCache;
    private readonly ILogger _logger = logger;
    public async Task<Result<IEnumerable<RoomResponse>>> GetAllRoomsAsync(CancellationToken cancellationToken = default)
    {
        var rooms = await _context.Rooms
            .AsNoTracking()
            .Where(r => !r.IsDeleted)             
            .Include(r => r.Department)           
            .OrderBy(r => r.RoomNumber)
            .ToListAsync(cancellationToken);

        return Result.Success(rooms.Select(MapToRoomResponse));
    }

    public async Task<Result<RoomResponse>> GetRoomByIdAsync(int roomId,CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms
            .AsNoTracking()
            .Where(r => r.Id == roomId && !r.IsDeleted) 
            .Include(r => r.Department)
            .FirstOrDefaultAsync(cancellationToken);
        if (room is null)
            return Result.Failure<RoomResponse>(RoomErrors.NotFound);

        return Result.Success(MapToRoomResponse(room));
    }

    public async Task<Result<RoomResponse>> CreateRoomAsync(int departmentId,RoomRequest request,CancellationToken cancellationToken = default)
    {
        // 1. Validate department exists
        var departmentExists = await _context.Departments
            .AnyAsync(d => d.Id == departmentId && !d.IsDeleted, cancellationToken);
        if (!departmentExists)
            return Result.Failure<RoomResponse>(RoomErrors.DepartmentNotFound);

        // 2. Prevent duplicate room number within the same department
        var isDuplicate = await _context.Rooms
            .AnyAsync(r =>
                r.DepartmentId == departmentId &&
                r.RoomNumber == request.RoomNumber &&
               !r.IsDeleted,cancellationToken);
        if (isDuplicate)
            return Result.Failure<RoomResponse>(RoomErrors.RoomNumberAlreadyExists);

        // 3. Persist
        var room = new Room
        {
            RoomNumber = request.RoomNumber,
            Type = request.Type,
            PricePerDay = request.PricePerDay,
            IsOccupied = false,
            LastOpen = DateTime.UtcNow,
            DepartmentId = departmentId
        };

        await _context.Rooms.AddAsync(room, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // 4. Re-fetch with navigation properties for the response
        var created = await _context.Rooms
            .AsNoTracking()
            .Include(r => r.Department)
            .FirstAsync(r => r.Id == room.Id, cancellationToken);

        return Result.Success(MapToRoomResponse(created));
    }

    public async Task<Result<AssignRoomResponse>> AssignRoomToAppointmentAsync(int roomId,int patientId,int appointmentId,AssignRoomRequest request,CancellationToken cancellationToken = default)
    {
        // 1. Validate room exists and is not deleted
        var room = await _context.Rooms
            .Where(r => r.Id == roomId && !r.IsDeleted)
            .Include(r => r.Department)
            .FirstOrDefaultAsync(cancellationToken);
        if (room is null)
            return Result.Failure<AssignRoomResponse>(RoomErrors.NotFound);

        // 2. Validate room is not already occupied
        if (room.IsOccupied)
            return Result.Failure<AssignRoomResponse>(RoomErrors.RoomAlreadyOccupied);

        // 3. Validate patient exists
        var patient = await _context.Patients
            .Where(p => p.Id == patientId && !p.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
        if (patient is null)
            return Result.Failure<AssignRoomResponse>(RoomErrors.PatientNotFound);

        // 4. Validate appointment exists and belongs to this patient
        var appointment = await _context.Appointments
            .Where(a => a.Id == appointmentId && a.PatientId == patientId && !a.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
        if (appointment is null)
            return Result.Failure<AssignRoomResponse>(RoomErrors.AppointmentNotFound);

        // 5. Guard: this appointment must not already have a room assigned
        var alreadyAssigned = await _context.PatientRooms
            .AnyAsync(pr =>pr.AppointmentId == appointmentId &&!pr.IsDeleted,cancellationToken);
        if (alreadyAssigned)
            return Result.Failure<AssignRoomResponse>(RoomErrors.AppointmentAlreadyHasRoom);

        // 6. Create the PatientRoom record
        var patientRoom = new PatientRoom
        {
            PatientId = patientId,
            RoomId = roomId,
            AppointmentId = appointmentId,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate
        };

        // 7. Mark the room as occupied
        room.IsOccupied = true;
        room.LastOpen = DateTime.UtcNow;

        await _context.PatientRooms.AddAsync(patientRoom, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new AssignRoomResponse(
            PatientRoomId: patientRoom.Id,
            PatientName: patient.Name,
            RoomNumber: room.RoomNumber,
            RoomType: room.Type,
            DepartmentName: room.Department.Name,
            AppointmentDate: appointment.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
            SlotDisplay: $"{appointment.AppointmentDate.Hour:D2}:00 – {appointment.AppointmentDate.Hour + 1:D2}:00",
            CheckInDate: patientRoom.CheckInDate.ToString("yyyy-MM-dd HH:mm"),
            CheckOutDate: patientRoom.CheckOutDate?.ToString("yyyy-MM-dd HH:mm")
        ));
    }
    public async Task<Result<DoctorAppointmentRoomResponse>> GetDoctorAppointmentRoomAsync(int doctorId,CancellationToken cancellationToken = default)
    {
        var doctor = await _context.Doctors
            .AsNoTracking()
            .Where(d => d.Id == doctorId && !d.IsDeleted)
            .Include(d => d.Department)
            .Include(d => d.Appointments.Where(a => !a.IsDeleted))          // ✅ active appointments only
                .ThenInclude(a => a.Patient)                                 // ✅ patient data per appointment
            .Include(d => d.Appointments.Where(a => !a.IsDeleted))
                .ThenInclude(a => a.PatientRooms.Where(pr => !pr.IsDeleted)) // ✅ active room assignments
                    .ThenInclude(pr => pr.Room)
                        .ThenInclude(r => r.Department)                // ✅ room's department
            .FirstOrDefaultAsync(cancellationToken);

        if (doctor is null)
            return Result.Failure<DoctorAppointmentRoomResponse>(RoomErrors.DoctorNotFound);

        var appointmentResponses = doctor.Appointments
            .OrderBy(a => a.AppointmentDate)
            .Select(a =>
            {
                // pick the first active room assignment for this appointment (if any)
                var patientRoom = a.PatientRooms.FirstOrDefault(pr => !pr.IsDeleted);

                return new AppointmentWithPatientRoomResponse(
                    AppointmentId: a.Id,
                    AppointmentDate: a.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
                    SlotDisplay: $"{a.AppointmentDate.Hour:D2}:00 – {a.AppointmentDate.Hour + 1:D2}:00",
                    Status: a.Status,
                    PatientName: a.Patient?.Name ?? "Unknown",
                    ReasonForVisit: a.ReasonForVisit,
                    Notes: a.Notes,
                    Room: patientRoom is null ? null : new RoomInfo(
                        RoomId: patientRoom.Room.Id,
                        RoomNumber: patientRoom.Room.RoomNumber,
                        Type: patientRoom.Room.Type,
                        PricePerDay: patientRoom.Room.PricePerDay,
                        DepartmentName: patientRoom.Room.Department?.Name ?? "Unknown"
                    )
                );
            })
            .ToList();

        return Result.Success(new DoctorAppointmentRoomResponse(
            DoctorId: doctor.Id,
            DoctorName: doctor.Name,
            Specialization: doctor.Specialization,
            DepartmentName: doctor.Department?.Name ?? "Unknown",
            Appointments: appointmentResponses
        ));
    }

    public async Task<Result<PatientAppointmentRoomResponse>> GetPatientAppointmentRoomAsync(int patientId,CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .Where(p => p.Id == patientId && !p.IsDeleted)
            .Include(p => p.Appointments.Where(a => !a.IsDeleted))           // ✅ active appointments only
                .ThenInclude(a => a.Doctor)                                   // ✅ doctor data per appointment
            .Include(p => p.Appointments.Where(a => !a.IsDeleted))
                .ThenInclude(a => a.PatientRooms.Where(pr => !pr.IsDeleted)) // ✅ active room assignments
                    .ThenInclude(pr => pr.Room)
                        .ThenInclude(r => r.Department)                       // ✅ room's department
            .FirstOrDefaultAsync(cancellationToken);

        if (patient is null)
            return Result.Failure<PatientAppointmentRoomResponse>(RoomErrors.PatientNotFound);

        var appointmentResponses = patient.Appointments
            .OrderBy(a => a.AppointmentDate)
            .Select(a =>
            {
                var patientRoom = a.PatientRooms.FirstOrDefault(pr => !pr.IsDeleted);

                return new AppointmentWithDoctorRoomResponse(
                    AppointmentId: a.Id,
                    AppointmentDate: a.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
                    SlotDisplay: $"{a.AppointmentDate.Hour:D2}:00 – {a.AppointmentDate.Hour + 1:D2}:00",
                    Status: a.Status,
                    DoctorName: a.Doctor?.Name ?? "Unknown",
                    Specialization: a.Doctor?.Specialization ?? null,
                    ReasonForVisit: a.ReasonForVisit,
                    Notes: a.Notes,
                    Room: patientRoom is null ? null : new RoomInfo(
                        RoomId: patientRoom.Room.Id,
                        RoomNumber: patientRoom.Room.RoomNumber,
                        Type: patientRoom.Room.Type,
                        PricePerDay: patientRoom.Room.PricePerDay,
                        DepartmentName: patientRoom.Room.Department?.Name ?? "Unknown"
                    )
                );
            })
            .ToList();

        return Result.Success(new PatientAppointmentRoomResponse(
            PatientId: patient.Id,
            PatientName: patient.Name,
            Gender: patient.Gender,
            Appointments: appointmentResponses
        ));
    }

    /*public async Task<Result> DeleteRoomAsync(int roomId,CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms
            .FindAsync([roomId], cancellationToken);

        if (room is null)
            return Result.Failure(RoomErrors.NotFound);

        if (room.IsDeleted)         // idempotent — already deleted
            return Result.Success();

        room.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    /*public async Task<Result> ToggleRoomStatusAsync(int roomId,CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms
            .FindAsync([roomId], cancellationToken);

        if (room is null)
            return Result.Failure(RoomErrors.NotFound);

        room.IsDeleted = !room.IsDeleted;   // flip
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    private static RoomResponse MapToRoomResponse(Room r) => new(
        Id: r.Id,
        RoomNumber: r.RoomNumber,
        Type: r.Type,
        PricePerDay: r.PricePerDay,
        IsOccupied: r.IsOccupied,
        LastOpen: r.LastOpen.ToString("yyyy-MM-dd HH:mm"),
        IsDeleted: r.IsDeleted,
        DepartmentId: r.DepartmentId,
        DepartmentName: r.Department?.Name ?? "Unknown"
    );
}*/

namespace Hospital_Management_System.Services;

public class RoomService(ApplicationDbContext context) : IRoomService
{
    private readonly ApplicationDbContext _context = context;
    private static readonly HashSet<string> BlockingStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Booked", "Confirmed", "Waiting", "Postponed", "Rescheduled"
    };
    
    // GET All Rooms :
    public async Task<Result<IEnumerable<RoomResponse>>> GetAllRoomsAsync(CancellationToken cancellationToken = default)
    {
        var rooms = await _context.Rooms
            .AsNoTracking()
            .Where(r => !r.IsDeleted)
            .ProjectToType<RoomResponse>()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<RoomResponse>>(rooms);
    }

    // GET Room By Id :
    public async Task<Result<RoomResponse>> GetRoomByIdAsync(int roomId, CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms
            .AsNoTracking()
            .Where(r => r.Id == roomId && !r.IsDeleted)
            .ProjectToType<RoomResponse>()
            .FirstOrDefaultAsync(cancellationToken);

        if (room is null)
            return Result.Failure<RoomResponse>(RoomErrors.NotFound);

        return Result.Success(room);
    }

    // GET Appointment in room :
    public async Task<Result<RoomAppointmentsResponse>> GetRoomAppointmentsAsync(int roomId, CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms
            .AsNoTracking()
            .Where(r => r.Id == roomId && !r.IsDeleted)
            .Include(r => r.Department)
            .Include(r => r.PatientRooms.Where(pr => !pr.IsDeleted))
                .ThenInclude(pr => pr.Appointment)
                    .ThenInclude(a => a.Doctor)
            .Include(r => r.PatientRooms.Where(pr => !pr.IsDeleted))
                .ThenInclude(pr => pr.Appointment)
                    .ThenInclude(a => a.Patient)
            .FirstOrDefaultAsync(cancellationToken);

        if (room is null)
            return Result.Failure<RoomAppointmentsResponse>(RoomErrors.NotFound);

        var appointments = room.PatientRooms
            .Select(pr => pr.Appointment)
            .Where(a => a != null && !a.IsDeleted)
            .OrderBy(a => a.AppointmentDate)
            .Select(a => new RoomAppointmentItem(
                Id: a.Id,
                DoctorId: a.DoctorId,
                DoctorName: a.Doctor?.Name ?? "Unknown",
                PatientId: a.PatientId,
                PatientName: a.Patient?.Name ?? "Unknown",
                AppointmentDate: a.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
                SlotDisplay: BuildSlotDisplay(a.AppointmentDate),
                Status: a.Status,
                Notes: a.Notes,
                ReasonForVisit: a.ReasonForVisit
            ))
            .ToList();

        return Result.Success(new RoomAppointmentsResponse(
            RoomId: room.Id,
            RoomNumber: room.RoomNumber,
            DepartmentId: room.DepartmentId,
            DepartmentName: room.Department?.Name ?? "Unknown",
            Type: room.Type,
            PricePerDay: room.PricePerDay,
            IsOccupied: room.IsOccupied,
            Appointments: appointments
        ));
    }

    // Create New Room
    public async Task<Result<RoomResponse>> CreateRoomAsync(int departmentId, RoomRequest request, CancellationToken cancellationToken = default)
    {
        var departmentExists = await _context.Departments
            .AnyAsync(d => d.Id == departmentId && !d.IsDeleted, cancellationToken);

        if (!departmentExists)
            return Result.Failure<RoomResponse>(RoomErrors.DepartmentNotFound);

        var isDuplicate = await _context.Rooms
            .AnyAsync(r => r.DepartmentId == departmentId && r.RoomNumber == request.RoomNumber && !r.IsDeleted, cancellationToken);

        if (isDuplicate)
            return Result.Failure<RoomResponse>(RoomErrors.RoomNumberAlreadyExists);

        var room = new Room
        {
            RoomNumber = request.RoomNumber,
            Type = request.Type,
            PricePerDay = request.PricePerDay,
            IsOccupied = false,
            LastOpen = DateTime.UtcNow,
            DepartmentId = departmentId
        };

        await _context.Rooms.AddAsync(room, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.Rooms
            .AsNoTracking()
            .Where(r => r.Id == room.Id)
            .ProjectToType<RoomResponse>()
            .FirstAsync(cancellationToken);

        return Result.Success(created);
    }

    // Add Appointment for Room :
    public async Task<Result<AssignRoomResponse>> AssignRoomToAppointmentAsync(int roomId, int patientId, int appointmentId, AssignRoomRequest request, CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms
            .Where(r => r.Id == roomId && !r.IsDeleted)
            .Include(r => r.Department)
            .FirstOrDefaultAsync(cancellationToken);

        if (room is null)
            return Result.Failure<AssignRoomResponse>(RoomErrors.NotFound);

        var patient = await _context.Patients
            .Where(p => p.Id == patientId && !p.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (patient is null)
            return Result.Failure<AssignRoomResponse>(RoomErrors.PatientNotFound);

        var appointment = await _context.Appointments
            .Where(a => a.Id == appointmentId && !a.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (appointment is null)
            return Result.Failure<AssignRoomResponse>(RoomErrors.AppointmentNotFound);

        if (appointment.PatientId != patientId)
            return Result.Failure<AssignRoomResponse>(RoomErrors.AppointmentTimeConflict);

        var alreadyAssigned = await _context.PatientRooms
            .AnyAsync(pr => pr.AppointmentId == appointmentId && !pr.IsDeleted, cancellationToken);

        if (alreadyAssigned)
            return Result.Failure<AssignRoomResponse>(RoomErrors.AppointmentAlreadyHasRoom);

        var (slotStart, slotEnd) = GetSlotBoundaries(appointment.AppointmentDate);

        var hasSlotConflict = await _context.PatientRooms
            .Where(pr => pr.RoomId == roomId && !pr.IsDeleted)
            .Include(pr => pr.Appointment)
            .AnyAsync(pr => BlockingStatuses.Contains(pr.Appointment.Status) &&
                            pr.Appointment.AppointmentDate >= slotStart &&
                            pr.Appointment.AppointmentDate < slotEnd, cancellationToken);

        if (hasSlotConflict)
            return Result.Failure<AssignRoomResponse>(RoomErrors.RoomSlotConflict);

        var checkInDate = appointment.AppointmentDate;
        var checkOutDate = checkInDate.AddDays(request.StayDurationDays);
        var totalCost = request.StayDurationDays * room.PricePerDay;

        var patientRoom = new PatientRoom
        {
            PatientId = patientId,
            RoomId = roomId,
            AppointmentId = appointmentId,
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDate,
            TotalCost = totalCost
        };

        room.IsOccupied = true;
        room.LastOpen = DateTime.UtcNow;

        await _context.PatientRooms.AddAsync(patientRoom, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new AssignRoomResponse(
            PatientRoomId: patientRoom.Id,
            PatientName: patient.Name,
            RoomNumber: room.RoomNumber,
            RoomType: room.Type,
            DepartmentName: room.Department.Name,
            AppointmentDate: appointment.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
            SlotDisplay: BuildSlotDisplay(appointment.AppointmentDate),
            CheckInDate: checkInDate.ToString("yyyy-MM-dd HH:mm"),
            CheckOutDate: checkOutDate.ToString("yyyy-MM-dd HH:mm"),
            TotalCost: totalCost,
            Notes: request.Notes
        ));
    }

    
    // GET Docote Appoinemtnet in that Room :
    public async Task<Result<DoctorAppointmentRoomResponse>> GetDoctorAppointmentRoomAsync(int doctorId, CancellationToken cancellationToken = default)
    {
       
        var doctor = await _context.Doctors
            .AsNoTracking()
            .Where(d => d.Id == doctorId && !d.IsDeleted)
            .Include(d => d.Department)
            .Include(d => d.Appointments.Where(a => !a.IsDeleted))
                .ThenInclude(a => a.Patient)
            .Include(d => d.Appointments.Where(a => !a.IsDeleted))
                .ThenInclude(a => a.PatientRooms.Where(pr => !pr.IsDeleted))
                    .ThenInclude(pr => pr.Room)
                        .ThenInclude(r => r.Department)
            .FirstOrDefaultAsync(cancellationToken);

        if (doctor is null)
            return Result.Failure<DoctorAppointmentRoomResponse>(RoomErrors.DoctorNotFound);

        var appointments = doctor.Appointments
            .OrderBy(a => a.AppointmentDate)
            .Select(a =>
            {
                var pr = a.PatientRooms.FirstOrDefault(x => !x.IsDeleted);
                return new AppointmentWithPatientRoomResponse(
                    AppointmentId: a.Id,
                    AppointmentDate: a.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
                    SlotDisplay: BuildSlotDisplay(a.AppointmentDate),
                    Status: a.Status,
                    PatientName: a.Patient?.Name ?? "Unknown",
                    ReasonForVisit: a.ReasonForVisit,
                    Notes: a.Notes,
                    Room: pr is null ? null : new RoomInfo(
                        RoomId: pr.Room.Id,
                        RoomNumber: pr.Room.RoomNumber,
                        Type: pr.Room.Type,
                        PricePerDay: pr.Room.PricePerDay,
                        DepartmentName: pr.Room.Department?.Name ?? "Unknown"
                    )
                );
            })
            .ToList();

        return Result.Success(new DoctorAppointmentRoomResponse(
            DoctorId: doctor.Id,
            DoctorName: doctor.Name,
            Specialization: doctor.Specialization,
            DepartmentName: doctor.Department?.Name ?? "Unknown",
            Appointments: appointments
        ));
    }

    // GET Patinemt Appoinemtnet in that Room :
    public async Task<Result<PatientAppointmentRoomResponse>> GetPatientAppointmentRoomAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .Where(p => p.Id == patientId && !p.IsDeleted)
            .Include(p => p.Appointments.Where(a => !a.IsDeleted))
                .ThenInclude(a => a.Doctor)
            .Include(p => p.Appointments.Where(a => !a.IsDeleted))
                .ThenInclude(a => a.PatientRooms.Where(pr => !pr.IsDeleted))
                    .ThenInclude(pr => pr.Room)
                        .ThenInclude(r => r.Department)
            .FirstOrDefaultAsync(cancellationToken);

        if (patient is null)
            return Result.Failure<PatientAppointmentRoomResponse>(RoomErrors.PatientNotFound);

        var appointments = patient.Appointments
            .OrderBy(a => a.AppointmentDate)
            .Select(a =>
            {
                var pr = a.PatientRooms.FirstOrDefault(x => !x.IsDeleted);
                return new AppointmentWithDoctorRoomResponse(
                    AppointmentId: a.Id,
                    AppointmentDate: a.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
                    SlotDisplay: BuildSlotDisplay(a.AppointmentDate),
                    Status: a.Status,
                    DoctorName: a.Doctor?.Name ?? "Unknown",
                    Specialization: a.Doctor?.Specialization ?? null,
                    ReasonForVisit: a.ReasonForVisit,
                    Notes: a.Notes,
                    Room: pr is null ? null : new RoomInfo(
                        RoomId: pr.Room.Id,                    
                        RoomNumber: pr.Room.RoomNumber,
                        Type: pr.Room.Type,
                        PricePerDay: pr.Room.PricePerDay,
                        DepartmentName: pr.Room.Department?.Name ?? "Unknown"
                    )
                );
            })
            .ToList();

        return Result.Success(new PatientAppointmentRoomResponse(
            PatientId: patient.Id,
            PatientName: patient.Name,
            Gender: patient.Gender,
            Appointments: appointments
        ));
    }

    // Deleted Room :
    public async Task<Result> DeleteRoomAsync(int roomId, CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms.FindAsync([roomId], cancellationToken);
        if (room is null) return Result.Failure(RoomErrors.NotFound);
        if (room.IsDeleted) return Result.Success();
        room.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // Toggle Room :
    public async Task<Result> ToggleRoomStatusAsync(int roomId, CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms.FindAsync([roomId], cancellationToken);
        if (room is null) return Result.Failure(RoomErrors.NotFound);
        room.IsDeleted = !room.IsDeleted;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }


    // Private Methods :
    private static (DateTime slotStart, DateTime slotEnd) GetSlotBoundaries(DateTime dt)
    {
        var start = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, DateTimeKind.Utc);
        return (start, start.AddHours(1));
    }

    private static string BuildSlotDisplay(DateTime dt)
        => $"{dt.Hour:D2}:00 – {dt.Hour + 1:D2}:00";
}
