namespace Hospital_Management_System.Entities;

public class Department : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } =string.Empty;
    public string Location { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;

    // RelationShip with Doctor: 
    public ICollection<Doctor> Doctors { get; set; } = [];
    // RelationShip with Patient :
    public ICollection<Patient> Patients { get; set; } = [];
    // RelationShip with Room :
    public ICollection<Room> Rooms { get; set; } = [];  

}
