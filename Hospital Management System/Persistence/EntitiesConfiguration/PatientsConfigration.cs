namespace Hospital_Management_System.Persistence.EntitiesConfigrations;
public class PatientsConfigration : IEntityTypeConfiguration<Patient>
{
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
                builder.HasKey(x => x.Id).HasName("Patients_PrimaryKey");
                builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
                
                builder.Property(p => p.MaxMedicalExpenses)
                    .HasColumnType("decimal(18,2)");

                builder.Property(x => x.PhoneNumber)
                    .HasMaxLength(20);
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
                                     "[NationalId] NOT LIKE '%[^0-9]%'")); // Only allow numeric values in NationalId


                builder.Property(x => x.CreatedBy).HasMaxLength(450); 
                builder.Property(x => x.ModifiedBy).HasMaxLength(450);
        //RelationShips
        // 1. One-to-Many: Patient -> Department
        builder.HasOne(p => p.Department)
               .WithMany(d => d.Patients)
               .HasForeignKey(p => p.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict);
   
    }


    /* One Patient Has Many Appointments
    builder.HasMany(x => x.Appointments)
           .WithOne(x => x.Patient!)
           .HasForeignKey(x => x.PatientId)
           .OnDelete(DeleteBehavior.Restrict);*/
}

