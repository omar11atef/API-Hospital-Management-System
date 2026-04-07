namespace Hospital_Management_System.Contracts.Authorization;

public class ForgatePasswrodRequestValidator: AbstractValidator<ForgatePasswrodRequest>
{
    public ForgatePasswrodRequestValidator()
    {
        RuleFor(x => x.Email)
           .NotEmpty()
           .EmailAddress();
    }
}
