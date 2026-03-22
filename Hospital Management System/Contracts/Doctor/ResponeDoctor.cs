namespace Hospital_Management_System.Contracts.Doctor;

public record ResponeDoctor
(
    int Id,
    string Name,
    string? Specialization,
    DateOnly DateOfBirth,
    string Gender,
    string? Email,
    string? Address,
    string AcademicDegree,
    string NationalId,
    string? PhoneNumber,
    bool IsDeleted,
    int DepartmentId
);

public record ResponseDoctorWithAppointments
(
    int DoctorId,
    string DoctorName,
    string DepartmentName, 
    IEnumerable<DoctorAppointment> Appointments
);

// New in Appoietment Service :
public sealed record DoctorAppointmentsResponse(
    int DoctorId,
    string DoctorName,
    string? Specialization,
    IReadOnlyList<AppointmentResponse> Appointments
);
