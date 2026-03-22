namespace Hospital_Management_System.Abstractions;

public record Error(string Code , string Descrpition)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}
