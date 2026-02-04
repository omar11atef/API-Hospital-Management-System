
namespace Hospital_Management_System.Services;

public interface IAuthorService
{
    Task<AuthorResponse?> IsAuthorCorrectAysun(string email, string password,CancellationTokenSource cancellationTokenSource);
}
