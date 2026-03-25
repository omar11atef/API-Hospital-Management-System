namespace Hospital_Management_System.Contracts.Room;

public record RoomRequest
(
    string RoomNumber,
    string Type,
    decimal PricePerDay
);

public sealed record AssignRoomRequest(
    int StayDurationDays,
    string? Notes
);
