namespace Hospital_Management_System.Abstractions.Consts;

public static class RegexPattern
{
    public const string Password = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$";

}
