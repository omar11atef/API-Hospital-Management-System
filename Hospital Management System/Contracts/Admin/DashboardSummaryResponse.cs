namespace Hospital_Management_System.Contracts.Admin;

public record DashboardSummaryResponse(
  
);
public record DailyDepartmentSummaryResponse(
        int TotalDoctors,
        int TodayAppointments,
        int TotalPatients 
 );

public record RoomOccupancyResponse(
        int TotalRooms,
        int OccupiedRooms,
        int AvailableRooms,
        double OccupancyRatePercentage 
);

public record TodayAppointmentsSummaryResponse(
        int TotalAppointments,
        int CompletedAppointments,
        int CancelledAppointments,
        int ActiveAppointments // (Booked, Confirmed, Waiting, etc.)
);
public record TopDoctorDto(int DoctorId, string DoctorName, string DepartmentName, int AppointmentCount);
public record TopDoctorsResponse(List<TopDoctorDto> TopDoctors);

