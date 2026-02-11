namespace Hospital_Management_System.Persistence.EntitiesConfigrations;
public class PatientsConfigration : IEntityTypeConfiguration<Patients>
{
        public void Configure(EntityTypeBuilder<Patients> builder)
        {
                builder.HasKey(x => x.Id).HasName("Patients_PrimaryKey");
                builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
                
                builder.Property(p => p.MaxMedicalExpenses)
                    .HasColumnType("decimal(18,2)");

                builder.Property(x => x.PhoneNumber)
                    .HasMaxLength(20);


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
                                     "[NationalId] NOT LIKE '%[^0-9]%'")); // Only allow numeric values in NationalId

        /*RelationShips
        // One Patient Has Many Appointments
        builder.HasMany(x => x.Appointments)
               .WithOne(x => x.Patient!)
               .HasForeignKey(x => x.PatientId)
               .OnDelete(DeleteBehavior.Restrict);*/
    }
}
