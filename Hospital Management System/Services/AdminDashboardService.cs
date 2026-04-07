using Microsoft.EntityFrameworkCore;

namespace Hospital_Management_System.Services;

public class AdminDashboardService(ApplicationDbContext context) : IAdminDashboardService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<DailyDepartmentSummaryResponse>> GetDailyDepartmentSummaryAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        // 1. Validate if the department actually exists
        var departmentExists = await _context.Departments.AnyAsync(d => d.Id == departmentId && !d.IsDeleted, cancellationToken);

        if (!departmentExists)
            return Result.Failure<DailyDepartmentSummaryResponse>(DepartmentErrors.NotFound);

        // Define "Today" boundaries (UTC)
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        // 2. Calculate Total Doctors in this department
        var totalDoctors = await _context.Doctors
            .CountAsync(d => d.DepartmentId == departmentId && !d.IsDeleted, cancellationToken);

        // 3. Calculate Today's Appointments for this department
        var todayAppointments = await _context.Appointments
            .CountAsync(a =>
                a.Doctor.DepartmentId == departmentId &&
                !a.IsDeleted &&
                a.AppointmentDate >= today &&
                a.AppointmentDate < tomorrow,
            cancellationToken);

        // 4. Calculate Total Unique Patients for this department
        // (Counts how many distinct patients have ever booked an appointment here)
        var totalPatients = await _context.Appointments
            .Where(a => a.Doctor.DepartmentId == departmentId && !a.IsDeleted)
            .Select(a => a.PatientId)
            .Distinct()
            .CountAsync(cancellationToken);

        // 5. Build and return the response
        var response = new DailyDepartmentSummaryResponse(
            TotalDoctors: totalDoctors,
            TodayAppointments: todayAppointments,
            TotalPatients: totalPatients
        );

        return Result.Success(response);
    }

    public async Task<Result<RoomOccupancyResponse>> GetRoomOccupancyRateAsync(CancellationToken cancellationToken = default)
    {
        var totalRooms = await _context.Rooms.CountAsync(r => !r.IsDeleted, cancellationToken);

        if (totalRooms == 0)
            return Result.Success(new RoomOccupancyResponse(0, 0, 0, 0));
        

        var occupiedRooms = await _context.Rooms.CountAsync(r => !r.IsDeleted , cancellationToken);

        var availableRooms = totalRooms - occupiedRooms ;

        double occupancyRate = Math.Round(((double)occupiedRooms / totalRooms) * 100, 2);

        var response = new RoomOccupancyResponse(
            TotalRooms: totalRooms,
            OccupiedRooms: occupiedRooms,
            AvailableRooms: availableRooms,
            OccupancyRatePercentage: occupancyRate
        );

        return Result.Success(response);
    }

    public async Task<Result<TodayAppointmentsSummaryResponse>> GetTodayAppointmentsSummaryAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var summary = await _context.Appointments
            .Where(a => !a.IsDeleted && a.AppointmentDate >= today && a.AppointmentDate < tomorrow)
            .GroupBy(a => 1) 
            .Select(g => new TodayAppointmentsSummaryResponse(
                g.Count(), 
                g.Count(a => a.Status == "Completed"),
                g.Count(a => a.Status == "Cancelled"),
                g.Count(a => a.Status != "Completed" && a.Status != "Cancelled") 
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (summary is null)
            return Result.Success(new TodayAppointmentsSummaryResponse(0, 0, 0, 0));
        
        return Result.Success(summary);
    }

    public async Task<Result<TopDoctorsResponse>> GetTopDoctorsAsync(int take, CancellationToken cancellationToken = default)
    {
        var topDoctorsList = await _context.Doctors
            .AsNoTracking()
            .Where(d => !d.IsDeleted)
            .OrderByDescending(d => d.Appointments.Count(a => !a.IsDeleted && a.Status != "Cancelled"))
            .Take(take) 
            .Select(d => new TopDoctorDto( 
                d.Id,
                d.Name,
                d.Department.Name,
                d.Appointments.Count(a => !a.IsDeleted && a.Status != "Cancelled")
            ))
            .ToListAsync(cancellationToken); 

        var response = new TopDoctorsResponse(topDoctorsList);

        return Result.Success(response);
    }
}
