namespace Hospital_Management_System.Contracts.DepartmentRequest;

public class DepartmentRequestValidation : AbstractValidator<DepartmentRequest>
{
    public DepartmentRequestValidation()
    {
        RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(200).WithMessage("Department name cannot exceed 200 characters.");

        RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(300).WithMessage("Location cannot exceed 300 characters.");

        RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number must be in a valid format.");
    }
}
