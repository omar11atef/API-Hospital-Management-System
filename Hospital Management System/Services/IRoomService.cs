using Hospital_Management_System.Contracts.Room;

namespace Hospital_Management_System.Services;

public interface IRoomService
{

    /*Task<Result<IEnumerable<RoomResponse>>> GetAllRoomsAsync(CancellationToken cancellationToken = default);

    Task<Result<RoomResponse>> GetRoomByIdAsync(int roomId,CancellationToken cancellationToken = default);

    Task<Result<DoctorAppointmentRoomResponse>> GetDoctorAppointmentRoomAsync(int doctorId,CancellationToken cancellationToken = default);

    Task<Result<PatientAppointmentRoomResponse>> GetPatientAppointmentRoomAsync(int patientId,CancellationToken cancellationToken = default);

    Task<Result<RoomResponse>> CreateRoomAsync(int departmentId,RoomRequest request,CancellationToken cancellationToken = default);

    Task<Result<AssignRoomResponse>> AssignRoomToAppointmentAsync(int roomId,int patientId,int appointmentId,AssignRoomRequest request,CancellationToken cancellationToken = default);

    Task<Result> DeleteRoomAsync(int roomId,CancellationToken cancellationToken = default);

    Task<Result> ToggleRoomStatusAsync(int roomId,CancellationToken cancellationToken = default);*/

    Task<Result<IEnumerable<RoomResponse>>> GetAllRoomsAsync(CancellationToken cancellationToken = default);
    Task<Result<RoomAppointmentsResponse>> GetRoomAppointmentsAsync(int roomId, CancellationToken cancellationToken = default);

    Task<Result<RoomResponse>> GetRoomByIdAsync(int roomId, CancellationToken cancellationToken = default);

    Task<Result<DoctorAppointmentRoomResponse>> GetDoctorAppointmentRoomAsync(int doctorId, CancellationToken cancellationToken = default);

    Task<Result<PatientAppointmentRoomResponse>> GetPatientAppointmentRoomAsync(int patientId, CancellationToken cancellationToken = default);

    Task<Result<RoomResponse>> CreateRoomAsync(int departmentId, RoomRequest request, CancellationToken cancellationToken = default);

    Task<Result<AssignRoomResponse>> AssignRoomToAppointmentAsync(int roomId, int patientId, int appointmentId, AssignRoomRequest request, CancellationToken cancellationToken = default);

    Task<Result> DeleteRoomAsync(int roomId, CancellationToken cancellationToken = default);

    Task<Result> ToggleRoomStatusAsync(int roomId, CancellationToken cancellationToken = default);
}
