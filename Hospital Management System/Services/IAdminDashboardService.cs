namespace Hospital_Management_System.Services;

public interface IAdminDashboardService
{
    Task<Result<DailyDepartmentSummaryResponse>> GetDailyDepartmentSummaryAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<Result<RoomOccupancyResponse>> GetRoomOccupancyRateAsync(CancellationToken cancellationToken = default);
    Task<Result<TodayAppointmentsSummaryResponse>> GetTodayAppointmentsSummaryAsync(CancellationToken cancellationToken = default);
    Task<Result<TopDoctorsResponse>> GetTopDoctorsAsync(int take, CancellationToken cancellationToken = default);
}
