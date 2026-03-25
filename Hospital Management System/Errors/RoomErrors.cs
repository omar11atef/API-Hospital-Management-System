namespace Hospital_Management_System.Errors;

public class RoomErrors
{
    public static readonly Error NotFound =
        new("Room.NotFound", "No room was found with the specified Id.");

    public static readonly Error DepartmentNotFound =
        new("Room.DepartmentNotFound", "No department was found with the specified Id.");

    public static readonly Error PatientNotFound =
        new("Room.PatientNotFound", "No patient was found with the specified Id.");

    public static readonly Error AppointmentNotFound =
        new("Room.AppointmentNotFound", "No appointment was found with the specified Id.");

    public static readonly Error DoctorNotFound =
        new("Room.DoctorNotFound", "No doctor was found with the specified Id.");

    public static readonly Error RoomNumberAlreadyExists =
        new("Room.Duplicate", "A room with this number already exists in the department.");

    public static readonly Error RoomAlreadyOccupied =
        new("Room.AlreadyOccupied", "This room is currently occupied and cannot be assigned again.");

    public static readonly Error AppointmentAlreadyHasRoom =
        new("Room.AppointmentAlreadyHasRoom", "This appointment already has a room assigned. Use the update endpoint to change it.");

    public static readonly Error AlreadyDeleted =
        new("Room.AlreadyDeleted", "This room has already been deleted.");

    public static readonly Error AppointmentAlreadyCancelled =
        new("Room.AppointmentAlreadyCancelled",
            "This appointment is already cancelled.");

    public static readonly Error AppointmentAlreadyCompleted =
        new("Room.AppointmentAlreadyCompleted",
            "This appointment is already completed and cannot be cancelled.");
    public static readonly Error RoomSlotConflict =
       new("Room.SlotConflict",
           "This room already has an active appointment in the same time slot. " +
           "Cancel or complete the existing appointment first, or choose a different slot.");

    public static readonly Error AppointmentTimeConflict =
    new("Appointment.TimeConflict",
        "That already has an appointment scheduled during this time slot.");
}
