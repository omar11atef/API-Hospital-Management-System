namespace Hospital_Management_System.Entities;

public class PatientVisit : AuditableEntity
{
    public int Id { get; set; }
    public DateTime VisitDate { get; set; } = DateTime.Now;
    public string VisitType { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // relationship with patient (Foreign Key)
    public int PatientId { get; set; }
    public Patients Patient { get; set; } = null!;

    // relationship with Doctor
    public int? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
}
