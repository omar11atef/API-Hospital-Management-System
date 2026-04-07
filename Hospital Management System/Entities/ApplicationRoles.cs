namespace Hospital_Management_System.Entities;

public class ApplicationRoles : IdentityRole
{
     public bool IsDefault { get; set; }
    public bool IsDeleted { get; set; }
}
