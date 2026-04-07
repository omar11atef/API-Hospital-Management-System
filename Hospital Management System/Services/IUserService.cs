using Hospital_Management_System.Contracts.User;

namespace Hospital_Management_System.Services;

public interface IUserService
{
    //Task<Result<UserProfileResponse>> GetProfileAsync(IEnumerable<string> userIds);
    Task<Result<UserResponse>> GetByIdUserAsync(string id);
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<UserResponse>> AddNewUserAsync(CreateUserRequest request, CancellationToken cancellationToken);
    Task<Result<UserProfileResponse>> GetProfileAsync(string UserId);
    Task<Result> UpdateProfileAsync(string UserId, UpdateProfileRequest request);
    Task<Result> ChangePasswordAsync(string UserId, ChangePasswordRequset request);
    Task<Result> ToggleStatuesAsync(string UserId);
    Task<Result> UnlockAsync(string UserId);
}
