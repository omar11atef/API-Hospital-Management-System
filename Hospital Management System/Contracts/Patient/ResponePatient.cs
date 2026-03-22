namespace Hospital_Management_System.Contracts.Patient;

public record ResponePatient
(
      int id ,
      string Name,
      string Gender,
      DateOnly DateOfBirth,
      string Address,
      string PhoneNumber,
      string NationalId,
      decimal MaxMedicalExpenses,
      string DiseaseName,
      bool IsDeleted
);

public record UpdateExpensesRequest(decimal Amount);

public sealed record PatientAppointmentsResponse(
    int      PatientId,
    string   PatientName,
    string   DepartmentName,
    IEnumerable<PatientAppointment>  Appointments
);
