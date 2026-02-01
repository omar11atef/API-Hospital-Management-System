

using System;
using System.Globalization;

namespace Hospital_Management_System.Contracts.Validations;

public class DoctorRequestValidation :AbstractValidator<RequestDoctor>
{
    public DoctorRequestValidation()
    {
        RuleFor(x => x.NationalId)
            .NotEmpty()
            .Length(14).WithMessage("National ID must be exactly 14 characters.")
            .Matches("^[0-9]*$").WithMessage("National ID must contain numbers only.");
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number must be in a valid format.");
        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .Must(BeAValidAge)
            .WithMessage("{PropertyName} Is InValid ,Age must be at least 18 years old.");

        RuleFor(x=>x.CreatedAt)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .When(x => x.CreatedAt.HasValue); 
    }
    private bool BeAValidAge(DateOnly? dateOfBirth)
    {
        if (!dateOfBirth.HasValue)
            return false;

        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - dateOfBirth.Value.Year;

        if (dateOfBirth.Value > today.AddYears(-age))
            age--;

        return age >= 18;
    }


}
