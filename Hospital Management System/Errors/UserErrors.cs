namespace Hospital_Management_System.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new
        ("User.Invalide", "Invalid Email/Password");
    public static readonly Error UserNotFound = new
        ("User.UserNotFound", "that User is Not Found");
    public static readonly Abstractions.Error DuplicateUser =
     new("User.DuplicatedContect ", "That User is Already exists");

    public static readonly Abstractions.Error EmailNotConfirmed =
     new("User.EmailNotConfirmed ", "That Email is Not Confirmed");
    public static readonly Abstractions.Error UserNameAlreadyExists =
    new("User.UserNameAlreadyExists ", "That User Name Already Exists");

    public static readonly Abstractions.Error InvalidCode =
    new("User.InvalidCode ", "That code is Invalid");
    
    public static readonly Abstractions.Error EmailAlreadyConfirm =
    new("User.EmailAlreadyConfirm ", "That Email is Already Confirmation");
    public static readonly Error EmailRegisteredButNotConfirmed = new(
    "User.EmailNotConfirmed",
    "This email is already registered but not confirmed. Please check your inbox or request a new confirmation code to login.");
}
