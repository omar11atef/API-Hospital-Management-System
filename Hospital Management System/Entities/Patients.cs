using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Hospital_Management_System.Entities;

public enum Gender
{
    female = 0,
    male = 1,
}
public class Patients : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    public Gender Gender { get; set; }
    public decimal MaxMedicalExpenses { get; set; }
    public string DiseaseName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;

    public ICollection<PatientVisit> Visits { get; set; } = new List<PatientVisit>();

    //relastionship with Room :
    public ICollection<PatientRooms> PatientRooms { get; set; } = new List<PatientRooms>();
}
