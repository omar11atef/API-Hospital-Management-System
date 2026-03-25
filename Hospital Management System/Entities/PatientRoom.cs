namespace Hospital_Management_System.Entities;

public class PatientRoom : AuditableEntity
{
    public int Id { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public decimal TotalCost { get; set; }
    public bool IsDeleted { get; set; } = false;

    // FR: PatientVisit
    //public int RelatedVisitId { get; set; }
    //public PatientVisit RelatedVisit { get; set; } = default!;

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = default!;
    //FK: Room
    public int RoomId { get; set; }
    public Room Room { get; set; } = default!;
    //FK: Appointment
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = default!;
}