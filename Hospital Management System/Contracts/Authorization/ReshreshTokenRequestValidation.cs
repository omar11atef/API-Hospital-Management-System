namespace Hospital_Management_System.Contracts.Authorization;

public class ReshreshTokenRequestValidation : AbstractValidator<RefreshTokenRequest>
{
    public ReshreshTokenRequestValidation()
    {
        RuleFor(x => x.token).NotEmpty();
        RuleFor(x => x.refreshToken).NotEmpty();
    }
}
