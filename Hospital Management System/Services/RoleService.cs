using System.Data;

namespace Hospital_Management_System.Services;

public class RoleService(RoleManager<ApplicationRoles> rolesManger , ApplicationDbContext context) : IRoleService
{
    private readonly RoleManager<ApplicationRoles> _rolesManger = rolesManger;
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool?includeDisable=false, CancellationToken cancellationToken = default) =>
        await _rolesManger.Roles
            .Where(x => !x.IsDefault && (!x.IsDeleted || (includeDisable.HasValue && includeDisable.Value)))
            .ProjectToType<RoleResponse>()
            .ToListAsync(cancellationToken);
    public async Task<Result<RoleDetailResponse>> GetBuIdAsync(string id)
    {
        var role = await _rolesManger.FindByIdAsync(id);
        if (role is null)
            return Result.Failure<RoleDetailResponse>(RolesErrors.RoleNotFound);
        //2- Role Found select Permission in Here :

        var permissions = await _rolesManger.GetClaimsAsync(role);

        var response = new RoleDetailResponse(role.Id, role.Name!,role.IsDeleted,permissions.Select(x=>x.Value));

        return Result.Success(response);    

    }
    public async Task<Result<RoleDetailResponse>> AddRoleAsync(RoleRequest request)
    {
        // Role Include that Exists or not :
        var RoleIsExists = await _rolesManger.RoleExistsAsync(request.Name);
        if (RoleIsExists)
            return Result.Failure<RoleDetailResponse>(RolesErrors.DuplicateRole);

        // That Permission correct :
        var allowedPermission = Permissions.GetAllPermissions();
        if(request.Permissions.Except(allowedPermission).Any())
            return Result.Failure<RoleDetailResponse>(RolesErrors.InvalidPermission);

        // Not Exists so add that new role :
        var role = new ApplicationRoles
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        var result = await _rolesManger.CreateAsync(role);

        if(result.Succeeded)
        {
            var permissions = request.Permissions
                .Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
                });
            await _context.AddRangeAsync(permissions);
            await _context.SaveChangesAsync();

            var response = new RoleDetailResponse(role.Id, role.Name, role.IsDeleted, request.Permissions);

            return Result.Success(response);
        }
        var error = result.Errors.First();

        return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
    }
    public async Task<Result> UpdateRoleAsync(string id,RoleRequest request)
    {
        // Change that role for new name not inlcude in DB :
        var RoleIsExists = await _rolesManger.Roles.AnyAsync(x => x.Name == request.Name && x.Id != id);
        if (RoleIsExists)
            return Result.Failure(RolesErrors.DuplicateRole);

        // role Found or not :
        var role = await _rolesManger.FindByIdAsync(id);
        if (role is null)
            return Result.Failure<RoleDetailResponse>(RolesErrors.RoleNotFound);
    
        // That Permission correct :
        var allowedPermission = Permissions.GetAllPermissions();
        if (request.Permissions.Except(allowedPermission).Any())
            return Result.Failure(RolesErrors.InvalidPermission);

        // Not Exists so Update that role :
        role.Name = request.Name;

        var result = await _rolesManger.UpdateAsync(role);

        if (result.Succeeded)
        {
            var currentpermission = await _context.RoleClaims
                .Where(x => x.RoleId == id && x.ClaimType == Permissions.Type)
                .Select(x => x.ClaimValue)
                .ToListAsync();

            var newPermission = request.Permissions
                .Except(currentpermission)
                .Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
                });

            var removedpermission = currentpermission.Except(request.Permissions);

            await _context.RoleClaims
                .Where(x => x.RoleId == id && removedpermission.Contains(x.ClaimValue))
                .ExecuteDeleteAsync();

            await _context.AddRangeAsync(newPermission);
            await _context.SaveChangesAsync();

            return Result.Success();

        }
        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> ToogleRoleAsync (string id)
    {
        if (await _rolesManger.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetailResponse>(RolesErrors.RoleNotFound);
        role.IsDeleted =!role.IsDeleted;
        await _rolesManger.UpdateAsync(role);
        return Result.Success();
    }




}
