using Hospital_Management_System.Entities;
using Microsoft.Extensions.Caching.Hybrid;

namespace Hospital_Management_System.Services;

public class RoomService(ApplicationDbContext context , HybridCache hybridCache , ILogger<RoomService> logger) : IRoomService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<RoomService> _logger = logger;
    private readonly HybridCache _hybridCache = hybridCache;
    private const string cachePrefix = "RoomCaches";

    private static readonly HashSet<string> BlockingStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Booked", "Confirmed", "Waiting", "Postponed", "Rescheduled"
    };
    
    // GET All Rooms :
    public async Task<Result<IEnumerable<RoomResponse>>> GetAllRoomsAsync(CancellationToken cancellationToken = default)
    {
       
        var cachkey = $"{cachePrefix}";
        var rooms = await _hybridCache.GetOrCreateAsync<IEnumerable<RoomResponse>>(
            cachkey,
            async cachQuery => 
                    await _context.Rooms
                    .AsNoTracking()
                    .Where(r => !r.IsDeleted)
                    .ProjectToType<RoomResponse>()
                    .ToListAsync(cancellationToken)
            );
        //await _hybridCache.RemoveAsync(cachkey,cancellationToken);
        return Result.Success<IEnumerable<RoomResponse>>(rooms);
    }

    // GET Room By Id :
    public async Task<Result<RoomResponse>> GetRoomByIdAsync(int roomId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{cachePrefix}-{roomId}";
        var room = await _hybridCache.GetOrCreateAsync<RoomResponse?>(
        cacheKey,
        async cancel => await _context.Rooms
            .AsNoTracking()
            .Where(r => r.Id == roomId && !r.IsDeleted)
            .ProjectToType<RoomResponse>()
            .FirstOrDefaultAsync(cancel),
        cancellationToken: cancellationToken
        );

        if (room is null)
            return Result.Failure<RoomResponse>(RoomErrors.NotFound);

        return Result.Success(room);
    }

    // GET Appointment in room :
    public async Task<Result<RoomAppointmentsResponse>> GetRoomAppointmentsAsync(int roomId, CancellationToken cancellationToken = default)
    {
        //bool fetchedFromDatabase = false;
        var cacheKey = $"{cachePrefix}-{roomId}";
        var rawData = await _hybridCache.GetOrCreateAsync<RoomAppointmentsResponse?>(
        cacheKey,
        async CachingQuery =>
        {
            //fetchedFromDatabase = true;
            //_logger.LogWarning("Cache MISS: Fetching data from SQL Database for Room {roomId}", roomId);

            return await _context.Rooms
                .AsNoTracking()
                .Where(r => r.Id == roomId && !r.IsDeleted)
                .ProjectToType<RoomAppointmentsResponse>()
                .FirstOrDefaultAsync(cancellationToken);
        },
        cancellationToken: cancellationToken);

        if (rawData is null)
            return Result.Failure<RoomAppointmentsResponse>(RoomErrors.NotFound);
        //if (!fetchedFromDatabase)
        //{
        //    _logger.LogInformation("Cache HIT: Retrieved data successfully from Memory/Cache for Room {roomId}", roomId);
        //}
        var finalAppointments = rawData.Appointments
            .Select(a => a with
            {
                SlotDisplay = BuildSlotDisplay(DateTime.Parse(a.AppointmentDate))
            })
            .ToList();
        var finalResponse = rawData with { Appointments = finalAppointments };

        return Result.Success(finalResponse);
    }
    // GET Docote Appoinemtnet in that Room :
    public async Task<Result<DoctorAppointmentRoomResponse>> GetDoctorAppointmentRoomAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"doctor:appointments:{doctorId}";
        //bool fetchedFromDatabase = false;

        var response = await _hybridCache.GetOrCreateAsync(
            cacheKey,
            async cachQuery =>
            {
                // This block ONLY runs on cache MISS
                //fetchedFromDatabase = true;
                // _logger.LogWarning( "Cache MISS: Fetching data from SQL Database for Doctor {doctorId}",doctorId);

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
                    .FirstOrDefaultAsync(cachQuery);

                if (doctor is null)
                    return null;

                return new DoctorAppointmentRoomResponse(
                    doctor.Id,
                    doctor.Name,
                    doctor.Specialization,
                    doctor.Department?.Name ?? "Unknown",
                    doctor.Appointments
                        .OrderBy(a => a.AppointmentDate)
                        .Select(a =>
                        {
                            var pr = a.PatientRooms.FirstOrDefault(x => !x.IsDeleted);
                            return new AppointmentWithPatientRoomResponse(
                                a.Id,
                                a.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
                                BuildSlotDisplay(a.AppointmentDate),
                                a.Status,
                                a.Patient?.Name ?? "Unknown",
                                a.ReasonForVisit,
                                a.Notes,
                                pr is null ? null : new RoomInfo(
                                    pr.Room.Id,
                                    pr.Room.RoomNumber,
                                    pr.Room.Type,
                                    pr.Room.PricePerDay,
                                    pr.Room.Department?.Name ?? "Unknown"
                                )
                            );
                        }).ToList()
                );
            },
            cancellationToken: cancellationToken);

        //if (!fetchedFromDatabase)
        //{
        //    _logger.LogInformation( "Cache HIT: Retrieved data from cache for Doctor {doctorId}",doctorId);
        //}

        if (response is null)
            return Result.Failure<DoctorAppointmentRoomResponse>(RoomErrors.DoctorNotFound);

        return Result.Success(response);
    }

    // GET Patinemt Appoinemtnet in that Room :
    public async Task<Result<PatientAppointmentRoomResponse>> GetPatientAppointmentRoomAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"patient:appointments:{patientId}";
        //bool fetchedFromDatabase = false;

        var response = await _hybridCache.GetOrCreateAsync<PatientAppointmentRoomResponse?>(
            cacheKey,
            async ct =>
            {
                //fetchedFromDatabase = true;

                // _logger.LogWarning( "Cache MISS: Fetching data from SQL Database for Patient {patientId}",patientId);

                var patient = await _context.Patients
                    .AsNoTracking()
                    .Where(p => p.Id == patientId && !p.IsDeleted)
                    .Include(p => p.Appointments.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.Doctor)
                    .Include(p => p.Appointments.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.PatientRooms.Where(pr => !pr.IsDeleted))
                            .ThenInclude(pr => pr.Room)
                                .ThenInclude(r => r.Department)
                    .FirstOrDefaultAsync(ct);

                if (patient is null)
                    return null;

                var appointments = patient.Appointments
                    .OrderBy(a => a.AppointmentDate)
                    .Select(a =>
                    {
                        var pr = a.PatientRooms.FirstOrDefault(x => !x.IsDeleted);
                        return new AppointmentWithDoctorRoomResponse(
                            a.Id,
                            a.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
                            BuildSlotDisplay(a.AppointmentDate),
                            a.Status,
                            a.Doctor?.Name ?? "Unknown",
                            a.Doctor?.Specialization,
                            a.ReasonForVisit,
                            a.Notes,
                            pr is null ? null : new RoomInfo(
                                pr.Room.Id,
                                pr.Room.RoomNumber,
                                pr.Room.Type,
                                pr.Room.PricePerDay,
                                pr.Room.Department?.Name ?? "Unknown"
                            )
                        );
                    })
                    .ToList();

                return new PatientAppointmentRoomResponse(
                    patient.Id,
                    patient.Name,
                    patient.Gender,
                    appointments
                );
            },
            cancellationToken: cancellationToken,
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(30),
                LocalCacheExpiration = TimeSpan.FromMinutes(5)
            });

        //if (!fetchedFromDatabase)
        //{
        //    _logger.LogInformation( "Cache HIT: Retrieved data from cache for Patient {patientId}",patientId);
        //}

        if (response is null)
            return Result.Failure<PatientAppointmentRoomResponse>(RoomErrors.PatientNotFound);

        return Result.Success(response);
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

        var room = request.Adapt<Room>();
        room.DepartmentId = departmentId;
        room.IsOccupied = false;
        room.LastOpen = DateTime.UtcNow;

        await _context.Rooms.AddAsync(room, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _hybridCache.RemoveAsync(cachePrefix, cancellationToken);
        //_logger.LogInformation("Cache INVALIDATED: Cleared '{CacheKey}' because a new room (ID: {RoomId}) was added.", cachePrefix, room.Id);

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
            .Include(r => r.Department)
            .FirstOrDefaultAsync(r => r.Id == roomId && !r.IsDeleted, cancellationToken);
        if (room is null)
            return Result.Failure<AssignRoomResponse>(RoomErrors.NotFound);

        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == patientId && !p.IsDeleted, cancellationToken);
        if (patient is null)
            return Result.Failure<AssignRoomResponse>(RoomErrors.PatientNotFound);

        var appointment = await _context.Appointments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == appointmentId && !a.IsDeleted, cancellationToken);
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
        await _hybridCache.RemoveAsync(cachePrefix, cancellationToken);
        await _hybridCache.RemoveAsync($"{cachePrefix}-{roomId}", cancellationToken); 

        //_logger.LogInformation("Cache INVALIDATED: Cleared general list and specific cache for Room {RoomId} after new appointment assignment (PatientRoom ID: {PatientRoomId}).", roomId, patientRoom.Id);
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

    //Update Room Data :
    public async Task<Result<RoomResponse>> UpdateRoomAsync(int roomId, int departmentId, RoomRequest request, CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId && !r.IsDeleted, cancellationToken);

        if (room is null)
            return Result.Failure<RoomResponse>(RoomErrors.NotFound);

        if (room.DepartmentId != departmentId)
        {
            var departmentExists = await _context.Departments
                .AnyAsync(d => d.Id == departmentId && !d.IsDeleted, cancellationToken);

            if (!departmentExists)
                return Result.Failure<RoomResponse>(RoomErrors.DepartmentNotFound);
        }

        if (room.RoomNumber != request.RoomNumber || room.DepartmentId != departmentId)
        {
            var isDuplicate = await _context.Rooms
                .AnyAsync(r => r.DepartmentId == departmentId
                            && r.RoomNumber == request.RoomNumber
                            && !r.IsDeleted
                            && r.Id != roomId, cancellationToken);

            if (isDuplicate)
                return Result.Failure<RoomResponse>(RoomErrors.RoomNumberAlreadyExists);
        }

        request.Adapt(room);
        room.DepartmentId = departmentId;

        await _context.SaveChangesAsync(cancellationToken);

        await _hybridCache.RemoveAsync(cachePrefix, cancellationToken);
        await _hybridCache.RemoveAsync($"{cachePrefix}-{roomId}", cancellationToken);

        // _logger.LogInformation("Cache INVALIDATED: Cleared general list and specific cache for Room {RoomId} after Update.", roomId);

        var updated = await _context.Rooms
            .AsNoTracking()
            .Where(r => r.Id == room.Id)
            .ProjectToType<RoomResponse>()
            .FirstAsync(cancellationToken);

        return Result.Success(updated);
    }

    // Deleted Room :
    public async Task<Result> DeleteRoomAsync(int roomId, CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms.FindAsync([roomId], cancellationToken);
        if (room is null) return Result.Failure(RoomErrors.NotFound);
        if (room.IsDeleted) return Result.Success();
        room.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        await _hybridCache.RemoveAsync(cachePrefix, cancellationToken);
        await _hybridCache.RemoveAsync($"{cachePrefix}-{roomId}", cancellationToken);
       // _logger.LogInformation("Cache INVALIDATED: Cleared general list and specific cache for Room {RoomId} after Deletion.", roomId);
        return Result.Success();
    }

    // Toggle Room :
    /*public async Task<Result> ToggleRoomStatusAsync(int roomId, CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms.FindAsync([roomId], cancellationToken);
        if (room is null) return Result.Failure(RoomErrors.NotFound);
        room.IsDeleted = !room.IsDeleted;
        await _context.SaveChangesAsync(cancellationToken);
        await _hybridCache.RemoveAsync(cachePrefix, cancellationToken);
        await _hybridCache.RemoveAsync($"{cachePrefix}-{roomId}", cancellationToken);
        string newStatus = room.IsDeleted ? "Deleted" : "Restored";
       // _logger.LogInformation("Cache INVALIDATED: Cleared general list and specific cache for Room {RoomId} after Status Toggled to '{Status}'.", roomId, newStatus);
        return Result.Success();
    }*/


    // Private Methods :
    private static (DateTime slotStart, DateTime slotEnd) GetSlotBoundaries(DateTime dt)
    {
        var start = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, DateTimeKind.Utc);
        return (start, start.AddHours(1));
    }

    private static string BuildSlotDisplay(DateTime dt)
        => $"{dt.Hour:D2}:00 – {dt.Hour + 1:D2}:00";
}
