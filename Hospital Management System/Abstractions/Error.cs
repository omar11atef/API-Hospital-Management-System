namespace Hospital_Management_System.Abstractions;

public record Error(string Code , string Descrpition , int? StatusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty,null);
}
