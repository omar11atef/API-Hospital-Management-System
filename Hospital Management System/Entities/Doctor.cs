namespace Hospital_Management_System.Entities;

public abstract class BaseEntity 
{
    public int Id { get; set; }
    public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public bool IsDeleted { get; set; } = false;
}

public sealed class Doctor : BaseEntity 
{
    public string Name { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? Address { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? AcademicDegree { get; set; }
    public decimal TotalHoursWorked { get; set; }
    public string? PhoneNumber { get; set; }
    public string NationalId { get; set; } = null!;
}