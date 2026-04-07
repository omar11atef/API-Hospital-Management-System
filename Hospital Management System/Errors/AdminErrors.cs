namespace Hospital_Management_System.Errors;

public static class AdminErrors
{
    public static readonly Error AggregateDataByNull =
       new("Admin.AggregateDataByNull", "Cannot aggregate data using a null parameter.", StatusCodes.Status400BadRequest);
}