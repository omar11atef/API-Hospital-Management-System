namespace Hospital_Management_System.Persistence.EntitiesConfigrations;

public class PatientRoomConfiguration : IEntityTypeConfiguration<PatientRooms>
{
    public void Configure(EntityTypeBuilder<PatientRooms> builder)
    {
        // Primary Key
        builder.HasKey(pr => pr.Id);

        //(Patient -> PatientRooms)
        builder.HasOne(pr => pr.Patient)
              .WithMany(p => p.PatientRooms)
              .HasForeignKey(pr => pr.PatientId)
              .OnDelete(DeleteBehavior.Restrict);

        // (Room -> PatientRooms)
        builder.HasOne(pr => pr.Room)
              .WithMany(r => r.PatientRooms)
              .HasForeignKey(pr => pr.RoomId)
              .OnDelete(DeleteBehavior.Restrict);

    }
}
