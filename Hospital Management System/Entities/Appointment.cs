namespace Hospital_Management_System.Entities;

public sealed class Appointment : AuditableEntity
{
    public int Id { get; set; }
    public DateTime AppointmentDate { get; set; } = DateTime.UtcNow;
    public DateTime EndTime { get; set; }
    public string ReasonForVisit { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;

    // RelatioShips :
    // With Doctor 
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = default!;

    // With Patient 
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public ICollection<PatientRoom> PatientRooms { get; set; } = [];




    /*private static class AppointmentStatuses
    {
        public const string Booked = "Booked";
        public const string Confirmed = "Confirmed";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
        public const string Postponed = "Postponed";
        public const string Rescheduled = "Rescheduled";
        public const string Waiting = "Waiting";

        // This array acts as our single source of truth for validation
        public static readonly string[] AllStatuses =
        [
            Booked, Confirmed, Completed, Cancelled, Postponed, Rescheduled, Waiting
        ];

    }*/

}
