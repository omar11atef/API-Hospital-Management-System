namespace Hospital_Management_System.Contracts.Appointment;

public class RequestAppointmentValidation : AbstractValidator<RequestAppointment>
{
    public RequestAppointmentValidation()
    {

        RuleFor(x => x.AppointmentDate)
            .NotEmpty().WithMessage("Appointment date is required.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Appointment date must be in the future.");

        RuleFor(x => x.ReasonForVisit)
            .MaximumLength(200).WithMessage("Reason for visit cannot exceed 200 characters.");
    }

}

public sealed class AppointmentRequestValidator : AbstractValidator<AppointmentRequest>
{
    private static readonly string[] ValidStatuses =
    [
        "Booked", "Confirmed", "Completed",
        "Cancelled", "Postponed", "Rescheduled", "Waiting"
    ];

    public AppointmentRequestValidator()
    {
        RuleFor(x => x.AppointmentDate)
            .NotEmpty().WithMessage("AppointmentDate is required.")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("AppointmentDate cannot be in the past.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => ValidStatuses.Contains(s))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}.");

        RuleFor(x => x.ReasonForVisit)
            .NotEmpty().WithMessage("ReasonForVisit is required.")
            .MaximumLength(500).WithMessage("ReasonForVisit must not exceed 500 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.");
    }
}
