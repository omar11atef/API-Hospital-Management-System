namespace Hospital_Management_System.Entities;

public sealed class Doctor : AuditableEntity 
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? Address { get; set; }
    public string Gender { get; set; }=string.Empty;
    public string? Email { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? AcademicDegree { get; set; }
    public decimal TotalHoursWorked { get; set; }
    public string? PhoneNumber { get; set; }
    public string NationalId { get; set; } = null!;
    public bool IsDeleted { get; set; } = false;

    // RaletionShip     
    // Many Doctors have many Patients
    public ICollection<Patient> Patient { get; set; } = [];

    // One Doctor belongs to one Department
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = default!;

    // one Doctor has Many Appointment : 
    public ICollection<Appointment> Appointments { get; set; } = [];
}