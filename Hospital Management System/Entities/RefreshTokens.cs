namespace Hospital_Management_System.Entities;

[Owned]
public class RefreshTokens
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresON { get; set; }
    public DateTime CreateON { get; set; }
    public DateTime? ReokedON { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresON;
    public bool IsActive => ReokedON is null && !IsExpired;
}
