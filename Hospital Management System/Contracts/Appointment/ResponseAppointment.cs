namespace Hospital_Management_System.Contracts.Appointment;

public record ResponseAppointment
(
    int Id,
    int DoctorId,
    string DoctorName, 
    int PatientId,
    string PatientName, 
    DateTime AppointmentDate,
    DateTime EndTime,
    string ReasonForVisit,
    string Status,
    string Notes
);

public sealed record AppointmentResponse(
    int Id,
    string DoctorName,
    string PatientName,
    string AppointmentDate,   // formatted: "3/24/2026 09:00"
    string Status,
    string Notes,
    string ReasonForVisit,
    bool IsDeleted
);

