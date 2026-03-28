
namespace Hospital_Management_System.Services;

public interface IAuthorService
{
    Task<Result<AuthorResponse>> IsAuthorCorrectAysun(string email, string password,CancellationTokenSource cancellationTokenSource);
    Task<Result> RegisterAsync(RegisterRequestApp request, CancellationToken cancellationToken);
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default);
    Task<Result> ResendConfirmEmailAsync(ResendConfirmEmailRequest request, CancellationToken cancellationToken = default);
}
