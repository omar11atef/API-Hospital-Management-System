using Hospital_Management_System.Entities;

namespace Hospital_Management_System.Persistence.EntitiesConfiguration;

public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRoles>
{
    public void Configure(EntityTypeBuilder<ApplicationRoles> builder)
    {
        //Default Data
        builder.HasData([
            new ApplicationRoles
            {
                Id = DefaultRoles.AdminRoleId,
                Name = DefaultRoles.Admin,
                NormalizedName = DefaultRoles.Admin.ToUpper(),
                ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp
            },
            new ApplicationRoles
            {
                Id = DefaultRoles.MemberRoleId,
                Name = DefaultRoles.Member,
                NormalizedName = DefaultRoles.Member.ToUpper(),
                ConcurrencyStamp = DefaultRoles.MemberRoleConcurrencyStamp,
                IsDefault = true
            }
        ]);
    }
}
