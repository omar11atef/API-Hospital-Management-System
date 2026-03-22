namespace Hospital_Management_System.Errors;

public static class DoctorErrors
{
    public static readonly Error DoctorNotFound = new
        ("Not Found", "That Failde cannot Found");

    public static readonly Error DuplicateNationalId = new
        ("National Id Duplicate", " that Value National Id Has Already Stored");

    public static readonly Error DepartmentNotFound =
    new("Department.NotFound", "DepartmentId not found.");
}
