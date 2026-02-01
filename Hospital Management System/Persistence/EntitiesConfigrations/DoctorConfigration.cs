
namespace Hospital_Management_System.Persistence.EntitiesConfigrations;

public class DoctorConfigration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.NationalId).IsUnique();

        builder.Property(x => x.PhoneNumber)
                 .HasMaxLength(20)
                 .IsRequired();
    }
}
