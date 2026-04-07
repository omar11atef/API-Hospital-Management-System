namespace Hospital_Management_System.Authentication;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestApp>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty();

        RuleFor(x => x.UserName)
           .NotEmpty()
           .Length(3, 100);

        RuleFor(x => x.Password)
           .NotEmpty()
           .Matches(RegexPattern.Password);

        RuleFor(x => x.FirstName)
           .NotEmpty()
           .Length(3, 100);

        RuleFor(x => x.LastName)
           .NotEmpty()
           .Length(3, 100);

    }
}