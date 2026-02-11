using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.Entities;

public class Rooms : AuditableEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string? RoomNumber { get; set; }
    public string? Type { get; set; }
    public DateTime LastOpen { get; set; }
    public bool IsOccupied { get; set; }
    public decimal PricePerDay { get; set; }

    // Relationship With Partient
    public ICollection<PatientRooms>? PatientRooms { get; set; }
}
