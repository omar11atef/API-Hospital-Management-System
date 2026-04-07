namespace Hospital_Management_System.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new
        ("User.Invalide", "Invalid Email/Password", StatusCodes.Status401Unauthorized);

    public static readonly Error DisableUser = new
       ("User.Disable", "That User Is Disable ,pleas contect your administrator", StatusCodes.Status409Conflict);
    
    public static readonly Error UserNotFound = new
        ("User.UserNotFound", "that User is Not Found", StatusCodes.Status404NotFound);
    
    public static readonly Abstractions.Error DuplicateUser =
     new("User.DuplicatedContect ", "That User is Already exists", StatusCodes.Status409Conflict);
    
    public static readonly Abstractions.Error LockedUser =
     new("Locked.DuplicatedContect ", "That User is Locked now , try again after Expiration of the period", StatusCodes.Status401Unauthorized);

    public static readonly Abstractions.Error EmailNotConfirmed =
     new("User.EmailNotConfirmed ", "That Email is Not Confirmed", StatusCodes.Status401Unauthorized);
    public static readonly Abstractions.Error EmailDuplication =
    new("User.EmailDuplication ", "That Email is Duplicater", StatusCodes.Status401Unauthorized);

    public static readonly Abstractions.Error UserNameAlreadyExists =
    new("User.UserNameAlreadyExists ", "That User Name Already Exists" ,StatusCodes.Status409Conflict);

    public static readonly Abstractions.Error InvalidCode =
    new("User.InvalidCode ", "That code is Invalid", StatusCodes.Status401Unauthorized);
    
    public static readonly Error InvalidJwtToken =
        new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);
    
    public static readonly Error InvalidRefreshToken =
       new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

    public static readonly Abstractions.Error EmailAlreadyConfirm =
    new("User.EmailAlreadyConfirm ", "That Email is Already Confirmation", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidRoles =
       new("Permissions.InvalidRoles", "Permission is invalid.", StatusCodes.Status400BadRequest);

    public static readonly Error EmailRegisteredButNotConfirmed = new(
    "User.EmailNotConfirmed",
    "This email is already registered but not confirmed. Please check your inbox or request a new confirmation code to login.", 
    StatusCodes.Status401Unauthorized);
}
