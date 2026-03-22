namespace Hospital_Management_System.Contracts.Requests;

public record RequestDoctor
(
    string Name,
    string Specialization ,
    string Address,
    DateOnly? DateOfBirth,
    string Gender,
    string? Email,
    string AcademicDegree,
    string PhoneNumber ,
    string NationalId 
);

public record DoctorAppointment
(
    int AppointmentId,
    string PatientName,
    string AppointmentDate,
    string Status
);



