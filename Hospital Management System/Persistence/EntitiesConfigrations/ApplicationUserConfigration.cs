namespace Hospital_Management_System.Persistence.EntitiesConfigrations;

public class ApplicationUserConfigration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName)
            .HasMaxLength(50);
        builder.Property(u => u.LastName)
            .HasMaxLength(50);
    }
}
