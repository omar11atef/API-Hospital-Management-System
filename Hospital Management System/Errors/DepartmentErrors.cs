namespace Hospital_Management_System.Errors;

public static class DepartmentErrors
{
    public static readonly Error NotFound =
        new("Department.NotFound", "The department was not found.", StatusCodes.Status404NotFound);

    public static readonly Error DuplicateName =
        new("Department.DuplicateName", "A department with this name already exists.", StatusCodes.Status409Conflict);
}