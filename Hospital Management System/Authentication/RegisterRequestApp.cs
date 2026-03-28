namespace Hospital_Management_System.Authentication;

public record RegisterRequestApp
(
    string Email ,
    string UserName ,
    string Password,
    string FirstName ,
    string LastName
);
