namespace Hospital_Management_System.Errors;

public static class PatientErrors
{
    public static readonly Error PatientNotFound =
        new("Patient.NotFound", "That patient cannot be found.", StatusCodes.Status404NotFound);

    public static readonly Error DuplicateNationalId =
        new("Patient.DuplicateNationalId", "Sorry: that National Id has already been stored.", StatusCodes.Status409Conflict);

    public static readonly Error InvalidAmount =
        new("Patient.InvalidAmount", "Amount must be greater than zero.", StatusCodes.Status400BadRequest);

    public static readonly Error DepartmentNotFound =
        new("Department.NotFound", "DepartmentId not found.", StatusCodes.Status404NotFound);
}
