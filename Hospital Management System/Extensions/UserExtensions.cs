namespace Hospital_Management_System.Extensions;

public static class UserExtensions
{
    public static string? GetUserId(this ClaimsPrincipal user) =>
      user.FindFirstValue(ClaimTypes.NameIdentifier);


    /*public static List<string> GetUserId(this ClaimsPrincipal user)
    {
        return user.FindAll(ClaimTypes.NameIdentifier)
                   .Select(claim => claim.Value)
                   .ToList();
    }*/
}
