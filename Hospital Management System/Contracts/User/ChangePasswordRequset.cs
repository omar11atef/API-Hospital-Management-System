namespace Hospital_Management_System.Contracts.User;

public record ChangePasswordRequset(
    string CurrentPassword,
    string NewPassword
    
);
