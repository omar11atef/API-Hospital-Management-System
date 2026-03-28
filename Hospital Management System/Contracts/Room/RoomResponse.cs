namespace Hospital_Management_System.Contracts.Room;

public record RoomResponse
(
     int Id,
    string RoomNumber,
    string Type,
    decimal PricePerDay,
    bool IsOccupied,
    string LastOpen,         
    bool IsDeleted,
    int DepartmentId,
    string DepartmentName
);

public sealed record RoomInfo(
    int RoomId,
    string RoomNumber,
    string Type,
    decimal PricePerDay,
    string DepartmentName
);
public sealed record AssignRoomResponse(
    int PatientRoomId,
    string PatientName,
    string RoomNumber,
    string RoomType,
    string DepartmentName,
    string AppointmentDate,
    string SlotDisplay,
    string CheckInDate,
    string CheckOutDate,
    decimal TotalCost,
    string? Notes
);

public sealed record CancelAppointmentResponse(
    int AppointmentId,
    string PreviousStatus,
    string NewStatus,
    string AppointmentDate,
    string SlotDisplay,
    bool RoomFreed,
    string? FreedRoomNumber
);

public sealed record AppointmentWithPatientRoomResponse(
    int AppointmentId,
    string AppointmentDate, 
    string SlotDisplay,
    string Status,
    string PatientName,
    string ReasonForVisit,
    string? Notes,
    RoomInfo? Room
);

public sealed record DoctorAppointmentRoomResponse(
    int DoctorId,
    string DoctorName,
    string? Specialization,
    string DepartmentName,
    IReadOnlyList<AppointmentWithPatientRoomResponse> Appointments
);

public sealed record AppointmentWithDoctorRoomResponse(
    int AppointmentId,
    string AppointmentDate,
    string SlotDisplay,
    string Status,
    string DoctorName,
    string? Specialization,
    string ReasonForVisit,
    string? Notes,
    RoomInfo? Room
);

public sealed record PatientAppointmentRoomResponse(
    int PatientId,
    string PatientName,
    string Gender,
    IReadOnlyList<AppointmentWithDoctorRoomResponse> Appointments
);

public sealed record RoomAppointmentItem(
    int Id,
    int DoctorId,
    string DoctorName,
    int PatientId,
    string PatientName,
    string AppointmentDate,
    string SlotDisplay,
    string Status,
    string? Notes,
    string ReasonForVisit
);

public sealed record RoomAppointmentsResponse(
    int RoomId,
    string RoomNumber,
    int DepartmentId,
    string DepartmentName,
    string Type,
    decimal PricePerDay,
    bool IsOccupied,
    IReadOnlyList<RoomAppointmentItem> Appointments
);