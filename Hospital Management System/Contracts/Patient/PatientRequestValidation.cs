using Microsoft.EntityFrameworkCore;

namespace Hospital_Management_System.Contracts.Patient;

public class PatientRequestValidation : AbstractValidator<RequestPatient>
{
    public PatientRequestValidation()
    {
        RuleFor(x => x.NationalId)
         .NotEmpty()
         .Length(14).WithMessage("National ID must be exactly 14 characters.")
         .Matches("^[0-9]*$").WithMessage("National ID must contain numbers only.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number must be in a valid format.");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Select From Female=0 , Male =1");

    }

}

