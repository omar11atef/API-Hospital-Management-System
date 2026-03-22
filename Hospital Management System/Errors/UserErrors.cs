namespace Hospital_Management_System.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new
        ("User.Invalide", "Invalid Email/Password");
}
