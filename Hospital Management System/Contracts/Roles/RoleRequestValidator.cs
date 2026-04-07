namespace Hospital_Management_System.Contracts.Roles;

public class RoleRequestValidator : AbstractValidator<RoleRequest>
{
    public RoleRequestValidator()
    {

        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3,200);

        RuleFor(x => x.Permissions)
            .NotEmpty()
            .NotNull();
        RuleFor(x => x.Permissions)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("You cnanot Add Duplicate Permission for the same role")
            .When(x=>x.Permissions !=null);


    }

}