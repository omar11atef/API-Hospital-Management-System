
namespace Hospital_Management_System.Services;

public interface IAuthorService
{
    Task<Result<AuthorResponse>> IsAuthorCorrectAysun(string email, string password,CancellationTokenSource cancellationTokenSource);
}
