namespace Hospital_Management_System.Authentication;

public record ConfirmEmailRequest
(
    string UserId,
    string Code
);

