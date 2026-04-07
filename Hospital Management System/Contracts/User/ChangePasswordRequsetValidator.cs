namespace Hospital_Management_System.Contracts.User;

public class ChangePasswordRequsetValidator : AbstractValidator<ChangePasswordRequset>
{
    public ChangePasswordRequsetValidator()
    {
        RuleFor(x => x.CurrentPassword)
           .NotEmpty();

        RuleFor(x => x.NewPassword)
          .NotEmpty()
          .Matches(RegexPattern.Password)
          .WithMessage("Password should be 8 or more with letter , num , Upper And Lower")
          .NotEqual(x=>x.CurrentPassword)
          .WithMessage("password shouldn't not equal current password");


    }
}
