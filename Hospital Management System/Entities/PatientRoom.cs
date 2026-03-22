namespace Hospital_Management_System.Entities;

public class PatientRoom : AuditableEntity
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = default!;
    public int RoomId { get; set; }
    public Room Room { get; set; } = default!;
    public DateTime CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public decimal TotalCost { get; set; }
    public int? RelatedVisitId { get; set; }
    public PatientVisit RelatedVisit { get; set; } = default!;
}