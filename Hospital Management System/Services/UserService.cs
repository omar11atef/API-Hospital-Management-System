namespace Hospital_Management_System.Services;

public class UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager , IRoleService roleService) : IUserService
{
    private readonly ApplicationDbContext _context=context;
    private readonly IRoleService _roleService = roleService;
    private readonly UserManager<ApplicationUser> _userManager=userManager;

    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken)
    {

        var result = await (from u in _context.Users
                            join ur in _context.UserRoles
                            on u.Id equals ur.UserId
                            join r in _context.Roles
                            on ur.RoleId equals r.Id into roles
                            where !roles.Any(x => x.Name == DefaultRoles.Member)
                            select new
                            {
                                u.Id,
                                u.FirstName,
                                u.LastName,
                                u.Email,
                                u.IsDisable,
                                Roles = roles.Select(x => x.Name).ToList()
                            }
                )
                .GroupBy(x => new { x.Id, x.FirstName, x.LastName, x.Email, x.IsDisable, x.Roles })
                .Select(x => new UserResponse
                (
                    x.Key.Id,
                    x.Key.FirstName,
                    x.Key.LastName,
                    x.Key.Email,
                    x.Key.IsDisable,
                    x.SelectMany(x => x.Roles)
                ))
                .ToListAsync(cancellationToken);

        return result;
    }
    public async Task<Result<UserResponse>> GetByIdUserAsync(string id)
    {
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);

        var useRroles = await _userManager.GetRolesAsync(user);

        /*var response = new UserResponse
            (
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email!,
                user.IsDisable,
                useRroles
            );*/

        var response = (user, useRroles).Adapt<UserResponse>();
        return Result.Success(response); 
    }
    public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId)
    {
        var user = await _userManager.Users
            .Where(x => x.Id == userId)
            .ProjectToType<UserProfileResponse>()
            .SingleAsync();
        return Result.Success(user);
    }
    public async Task<Result<UserResponse>> AddNewUserAsync(CreateUserRequest request,CancellationToken cancellationToken)
    {
        var EmailExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email);
        if (EmailExist)
            return Result.Failure<UserResponse>(UserErrors.EmailDuplication);

        var allowedPermission = await _roleService.GetAllAsync(cancellationToken: cancellationToken);

        if (request.Roles.Except(allowedPermission.Select(x => x.Name)).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        var user = request.Adapt<ApplicationUser>();
        user.Email = request.Email;
        user.EmailConfirmed = true;

        var result = await _userManager.CreateAsync(user ,request.Password);
        if(result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, request.Roles);
            var response = (user, request.Roles).Adapt<UserResponse>();
            return Result.Success(response);
        }

        var error = result.Errors.First();
        return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }
    public async Task<Result> UpdateProfileAsync (string UserId, UpdateProfileRequest request)
    {
        /*var user = await _userManager.FindByIdAsync (UserId);
          user = request.Adapt(user);
          await _userManager.UpdateAsync(user!);*/

        // New update for best performance :
        await _userManager.Users
            .Where(x => x.Id == UserId)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.FirstName, request.FirstName)
                    .SetProperty(x => x.LastName, request.LastName)
            );

        return Result.Success();
    }
    public async Task<Result> ChangePasswordAsync(string UserId, ChangePasswordRequset request)
    {
        var user = await _userManager.FindByIdAsync(UserId);
        
        var result = await _userManager.ChangePasswordAsync(user!,request.CurrentPassword,request.NewPassword);
        if (result.Succeeded)
            return Result.Success();
        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code,error.Description,StatusCodes.Status401Unauthorized)) ;
    }
    public async Task<Result> ToggleStatuesAsync(string UserId)
    {
        //var userExist = _context.Users.Any(x => x.Id == UserId);
        var UserExist = await _userManager.FindByIdAsync(UserId);
        if (UserExist is null)
            return Result.Failure(UserErrors.UserNotFound);

        UserExist.IsDisable = !UserExist.IsDisable;
        var result = await _userManager.UpdateAsync(UserExist);
        if (result.Succeeded)
            return Result.Success();
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
    }
    public async Task<Result> UnlockAsync(string UserId)
    {
        //var userExist = _context.Users.Any(x => x.Id == UserId);
        var UserExist = await _userManager.FindByIdAsync(UserId);
        if (UserExist is null)
            return Result.Failure(UserErrors.UserNotFound);

        var result = await _userManager.SetLockoutEndDateAsync(UserExist,null);
        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
    }





}
