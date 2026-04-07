namespace Hospital_Management_System.Authentication;

public record ResendPasswordRequestApp(
    string Email,
    string Code,
    string NewPassword
);
