namespace Hospital_Management_System.Errors;

public static class PatientErrors
{
    public static readonly Error PatientNotFound = new
        ("Not Found", "That Failde cannot Found");
    public static readonly Error DuplicateNationalId = new
        ("National Id Duplicate", "Sorry: that Value National Id Has Already Stored");
    public static readonly Error InvalidAmount= new
        ("Patient.InvalidAmount", "Amount must be greater than zero.");
    public static readonly Error DepartmentNotFound =
            new("Department.NotFound", "DepartmentId not found.");
}
