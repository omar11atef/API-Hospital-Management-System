namespace Hospital_Management_System.Persistence.EntitiesConfiguration;

public class UserCongiration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.OwnsMany(x => x.RefreshTokens)
            .ToTable("RefershTokens")
            .WithOwner()
            .HasForeignKey("UserId");

        builder.Property(b => b.FirstName).HasMaxLength(100);
        builder.Property(b => b.LastName).HasMaxLength(100);
    }
}
