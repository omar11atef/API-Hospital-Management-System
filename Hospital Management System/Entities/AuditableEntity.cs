namespace Hospital_Management_System.Entities;

public class AuditableEntity
{
    //public string CreatedById { get; set; } = string.Empty;
    //public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    //public string? UpdatedById { get; set; }
    //public DateTime? UpdatedOn { get; set; }     
    //public ApplicationUser CreatedBy { get; set; } = default!;
    //public ApplicationUser? UpdatedBy { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    
}

