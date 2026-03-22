namespace Hospital_Management_System.Contracts.Patient;

public record RequestPatient 
(
      string Name,
      string Address,
      DateOnly DateOfBirth,
      string PhoneNumber,
      string Gender,
      string NationalId,
      decimal MaxMedicalExpenses,
      string DiseaseName
);

public record PatientAppointment
(
    int AppointmentId,
    string DoctorName,
    string AppointmentDate,
    string Status
);
