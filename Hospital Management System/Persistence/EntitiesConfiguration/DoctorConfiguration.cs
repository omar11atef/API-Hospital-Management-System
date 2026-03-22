
namespace Hospital_Management_System.Persistence.EntitiesConfigrations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PhoneNumber)
                 .HasMaxLength(20)
                 .IsRequired();

        builder.Property(x => x.Gender).HasMaxLength(10).IsRequired();


        // search by NationalId , should be unique:
        builder.HasIndex(p => p.NationalId)
                       .IsUnique();
        // Handling NationalId properties , Stored :
        builder.Property(p => p.NationalId)
               .IsFixedLength()
               .HasMaxLength(14)
               .IsUnicode(false);
        builder.ToTable(t =>
        t.HasCheckConstraint("CK_Patient_NationalId_Numeric",
                             "[NationalId] NOT LIKE '%[^0-9]%'"));

        builder.ToTable(t => t.HasCheckConstraint("CK_Doctor_NationalId_Numeric", "[NationalId] NOT LIKE '%[^0-9]%'"));

        builder
              .Property(d => d.TotalHoursWorked)
              .HasColumnType("decimal(18,2)")
              .HasDefaultValue(0.0m);

        builder.Property(x => x.CreatedBy).HasMaxLength(450);
        builder.Property(x => x.ModifiedBy).HasMaxLength(450);


        // RelationShip with Patient:

        // relationships with Department :
        builder.HasOne(x => x.Department)
               .WithMany(d => d.Doctors)
               .HasForeignKey(x => x.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict);

    }
}
