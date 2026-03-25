namespace Hospital_Management_System.Contracts.Appointment;

public record RequestAppointment
(
    int DoctorId,
    int PatientId,
    DateTime AppointmentDate, 
    DateTime EndTime,         
    string? ReasonForVisit,
    string? Notes
);

public sealed record AppointmentRequest(
    DateTime? AppointmentDate,
    string Status,
    string Notes,
    string ReasonForVisit
);





