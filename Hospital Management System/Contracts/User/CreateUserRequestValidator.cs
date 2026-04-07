namespace Hospital_Management_System.Contracts.User;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Roles)
            .NotEmpty()
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("You Can't Add duplicate role for the same user")
            .When(x => x.Roles != null);

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegexPattern.Password)
            .WithMessage("Password should be 8 or more with letter, num, Upper And Lower");
    }
}