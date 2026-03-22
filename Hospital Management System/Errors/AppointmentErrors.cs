namespace Hospital_Management_System.Errors;

public class AppointmentErrors
{
    public static readonly Error DoubleBooking = 
        new("Appointment.DoubleBooking", "The doctor is already booked at this specific time.");


    public static readonly Error NotFound =
     new("Appointment.NotFound", "The appointment with the specified ID was not found.");

    public static readonly Error DoctorNotFound =
        new("Appointment.DoctorNotFound", "The specified doctor does not exist or has been deleted.");

    public static readonly Error PatientNotFound =
        new("Appointment.PatientNotFound", "The specified patient does not exist or has been deleted.");

    public static readonly Error TimeConflict =
        new("Appointment.TimeConflict",
            "The doctor already has an appointment scheduled during this time slot.");

    public static readonly Error InvalidDateRange =
        new("Appointment.InvalidDateRange", "EndTime must be after AppointmentDate.");

    public static readonly Error AlreadyDeleted =
        new("Appointment.AlreadyDeleted", "This appointment has already been cancelled or deleted.");

    public static readonly Error ValidationFailed =
        new("Appointment.ValidationFailed", "One or more validation errors occurred.");
    public static readonly Error NoDeletedAppointmentsFound =
    new("Appointment.NoDeletedAppointmentsFound", "No deleted appointments were found.");
}
