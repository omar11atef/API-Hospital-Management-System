namespace Hospital_Management_System.Entities;

public class Room : AuditableEntity
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime LastOpen { get; set; }
    public bool IsOccupied { get; set; } = false;
    public decimal PricePerDay { get; set; } = decimal.Zero;
    public bool IsDeleted { get; set; } = false;

    // Relationship With Partient
    public ICollection<PatientRoom> PatientRooms { get; set; } = [];

    // RelationShip With Department :
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = default!;

    // RelationShip with Appointment :
   // public int AppointmentId { get; set; }
    //public Appointment Appointment { get; set; } = default!;
}
