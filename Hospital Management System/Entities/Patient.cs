
namespace Hospital_Management_System.Entities;

public class Patient : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    public string Gender { get; set; } = string.Empty;
    public decimal MaxMedicalExpenses { get; set; }
    public string DiseaseName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;


    public ICollection<PatientVisit> Visits { get; set; } = [];

    // relastionship with Doctor :
    public ICollection<Doctor> Doctor { get; set; } = [];

    // relastionship with Department :
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = default!;


    //relationship with Room :
    public ICollection<PatientRoom> PatientRooms { get; set; } = [];
    
    // relationship With Appointment : 
    public ICollection<Appointment> Appointments { get; set; } = [];
}
