namespace Hospital_Management_System.Contracts.Patient;

public record ResponePatient
(
      int id ,
      string Name,
      Gender Gender,
      DateOnly DateOfBirth,
      string Address,
      string PhoneNumber,
      string NationalId,
      decimal MaxMedicalExpenses,
      string DiseaseName,
      bool IsDeleted
);

public record UpdateExpensesRequest(decimal Amount);
