namespace Hospital_Management_System.Authentication;

public class ResendPasswordRequestValidator : AbstractValidator<ResendPasswordRequestApp>
{
    public ResendPasswordRequestValidator()
    {
        RuleFor(x=>x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x=>x.NewPassword)
            .NotEmpty()
            .Matches(RegexPattern.Password)
            .WithMessage("Password should be 8 or more with letter , num , Upper And Lower");
    }
}