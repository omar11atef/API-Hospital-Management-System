namespace Hospital_Management_System.Errors;

public static class AppointmentErrors
{
    public static readonly Error DoubleBooking =
        new("Appointment.DoubleBooking", "The doctor is already booked at this specific time.", StatusCodes.Status409Conflict);

    public static readonly Error NotFound =
        new("Appointment.NotFound", "The appointment with the specified ID was not found.", StatusCodes.Status404NotFound);

    public static readonly Error DoctorNotFound =
        new("Appointment.DoctorNotFound", "The specified doctor does not exist or has been deleted.", StatusCodes.Status404NotFound);

    public static readonly Error PatientNotFound =
        new("Appointment.PatientNotFound", "The specified patient does not exist or has been deleted.", StatusCodes.Status404NotFound);

    public static readonly Error TimeConflict =
        new("Appointment.TimeConflict", "The doctor already has an appointment scheduled during this time slot.", StatusCodes.Status409Conflict);

    public static readonly Error InvalidDateRange =
        new("Appointment.InvalidDateRange", "EndTime must be after AppointmentDate.", StatusCodes.Status400BadRequest);

    public static readonly Error AlreadyDeleted =
        new("Appointment.AlreadyDeleted", "This appointment has already been cancelled or deleted.", StatusCodes.Status409Conflict); // 410 Gone is also an acceptable alternative here

    public static readonly Error ValidationFailed =
        new("Appointment.ValidationFailed", "One or more validation errors occurred.", StatusCodes.Status400BadRequest);

    public static readonly Error NoDeletedAppointmentsFound =
        new("Appointment.NoDeletedAppointmentsFound", "No deleted appointments were found.", StatusCodes.Status404NotFound);

    public static readonly Error CannotBookInPast =
        new("Appointment.CannotBookInPast", "You cannot book an appointment in the past.", StatusCodes.Status400BadRequest);
}