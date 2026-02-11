namespace Hospital_Management_System.Entities;

public class PatientRooms : AuditableEntity
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patients Patient { get; set; } = null!;
    public int RoomId { get; set; }
    public Rooms Room { get; set; } = null!;
    public DateTime CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public decimal TotalCost { get; set; }
    public int? RelatedVisitId { get; set; }
    public PatientVisit? RelatedVisit { get; set; }
}