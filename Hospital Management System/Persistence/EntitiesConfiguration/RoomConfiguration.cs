namespace Hospital_Management_System.Persistence.EntitiesConfiguration;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(x => x.Id).HasName("PK_Rooms");

        builder.Property(x => x.RoomNumber)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(x => x.RoomNumber)
               .IsUnique()
               .HasFilter("[IsDeleted] = 0"); 

        builder.Property(x => x.Type)
               .HasMaxLength(50);

        builder.Property(x => x.PricePerDay)
               .HasColumnType("decimal(18,2)")
               .HasDefaultValue(0m);

        builder.HasQueryFilter(x => x.IsDeleted == false);

        // (Relationships)

        // (Room -> Department)
        builder.HasOne(r => r.Department)
               .WithMany(d => d.Rooms)
               .HasForeignKey(r => r.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
