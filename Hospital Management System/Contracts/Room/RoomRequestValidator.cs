namespace Hospital_Management_System.Contracts.Room;

public class RoomRequestValidator : AbstractValidator<RoomRequest>
{
    private static readonly string[] ValidTypes =
     [
         "General", "Private", "ICU", "Emergency",
        "Operating", "Recovery", "Pediatric", "Maternity"
     ];

    public RoomRequestValidator()
    {
        RuleFor(x => x.RoomNumber)
            .NotEmpty().WithMessage("RoomNumber is required.")
            .MaximumLength(20).WithMessage("RoomNumber must not exceed 20 characters.")
            .Matches(@"^[A-Za-z0-9\-]+$")
            .WithMessage("RoomNumber may only contain letters, numbers, and hyphens.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required.")
            .Must(t => ValidTypes.Contains(t))
            .WithMessage($"Type must be one of: {string.Join(", ", ValidTypes)}.");

        RuleFor(x => x.PricePerDay)
            .GreaterThan(0).WithMessage("PricePerDay must be greater than zero.")
            .LessThanOrEqualTo(100_000).WithMessage("PricePerDay maximum is 100,000.");
    }
}

public sealed class AssignRoomRequestValidator : AbstractValidator<AssignRoomRequest>
{
    public AssignRoomRequestValidator()
    {
        RuleFor(x => x.StayDurationDays)
              .GreaterThan(0).WithMessage("StayDurationDays must be at least 1.")
              .LessThanOrEqualTo(365).WithMessage("StayDurationDays cannot exceed 365.");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.")
            .When(x => x.Notes is not null);
    }
}  
 

