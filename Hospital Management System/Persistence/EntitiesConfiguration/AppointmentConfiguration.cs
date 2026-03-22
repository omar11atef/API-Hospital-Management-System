namespace Hospital_Management_System.Persistence.EntitiesConfiguration;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        // 1. Primary Key
        builder.HasKey(a => a.Id).HasName("PK_Appointments");

        // 2. Properties
        builder.Property(a => a.ReasonForVisit)
               .HasMaxLength(250);

        builder.Property(a => a.Notes)
               .HasMaxLength(500);

        builder.Property(a => a.Status)
               .IsRequired()
               .HasMaxLength(50); 

        builder.Property(a => a.IsDeleted)
               .HasDefaultValue(false);

        // Global Query Filter 
        builder.HasQueryFilter(a => a.IsDeleted == false);

        // note Doctor has duplicate Date
        builder.HasIndex(a => new { a.DoctorId, a.AppointmentDate })
               .IsUnique()
               .HasFilter("[IsDeleted] = 0")
               .HasDatabaseName("IX_Unique_Doctor_AppointmentTime");

        // 5. Relationships
        // With Doctors
        builder.HasOne(a => a.Doctor)
               .WithMany(p => p.Appointments)
               .HasForeignKey(a => a.DoctorId);


        // with Patient :
        builder.HasOne(a => a.Patient)
               .WithMany(p => p.Appointments)
               .HasForeignKey(a => a.PatientId);
               
    }
}
