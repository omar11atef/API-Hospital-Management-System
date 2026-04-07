using Hospital_Management_System.Contracts.Roles;

namespace Hospital_Management_System.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisable = false, CancellationToken cancellationToken=default);
    Task<Result<RoleDetailResponse>> GetBuIdAsync(string id);
    Task<Result<RoleDetailResponse>> AddRoleAsync(RoleRequest request);
    Task<Result> UpdateRoleAsync(string id, RoleRequest request);
    Task<Result> ToogleRoleAsync(string id);
}
