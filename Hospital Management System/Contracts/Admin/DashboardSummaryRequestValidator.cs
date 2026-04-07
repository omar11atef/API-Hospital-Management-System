namespace Hospital_Management_System.Contracts.Admin;

public class DashboardSummaryRequestValidator : AbstractValidator<DashboardSummaryRequest>
{
    public DashboardSummaryRequestValidator()
    {
        
    }

    public class RoomOccupancyValidator : AbstractValidator<RoomOccupancyRequest>
    {
        public RoomOccupancyValidator()
        {
        }
    }

    public class TopDoctorsValidator : AbstractValidator<TopDoctorsRequest>
    {
        public TopDoctorsValidator()
        {
            RuleFor(x => x.Take)
                .InclusiveBetween(1, 50)
                .WithMessage("The 'take' parameter must be between 1 and 50.");
        }
    }
}


