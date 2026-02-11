namespace Hospital_Management_System.Contracts.Patient;

public record RequestPatient 
(
      string Name,
      string Address,
      DateOnly DateOfBirth,
      string PhoneNumber,
      Gender Gender,
      string NationalId,
      decimal MaxMedicalExpenses,
      string DiseaseName
);
