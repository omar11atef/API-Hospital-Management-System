using Hospital_Management_System.Entities;

namespace Hospital_Management_System.Persistence.EntitiesConfigrations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(x => x.Id).HasName("PK_Departments");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Location).HasMaxLength(300);
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.HasIndex(x => x.Name);
    }
}
