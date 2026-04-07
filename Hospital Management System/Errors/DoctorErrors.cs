namespace Hospital_Management_System.Errors;

public static class DoctorErrors
{
    public static readonly Error DoctorNotFound =
        new("Doctor.NotFound", "That doctor cannot be found.", StatusCodes.Status404NotFound);

    public static readonly Error DuplicateNationalId =
        new("Doctor.DuplicateNationalId", "Sorry: that National Id has already been stored.", StatusCodes.Status409Conflict);

    public static readonly Error DepartmentNotFound =
        new("Department.NotFound", "DepartmentId not found.", StatusCodes.Status404NotFound);
}
