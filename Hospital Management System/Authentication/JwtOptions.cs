namespace Hospital_Management_System.Authentication;

public class JwtOptions
{
    public static string OptionName = "Jwt";
    public string key { get; set; } = string.Empty;
    public string issuer { get; set; } = string.Empty;
    public string audience { get; set; } = string.Empty;
    public int expires { get; set; }
}
