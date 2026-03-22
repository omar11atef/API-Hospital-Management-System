namespace Hospital_Management_System.Contracts.DepartmentRequest;

public record DepartmentResponse
(
    int Id,
    string Name,
    string Location,
    string PhoneNumber,
    bool IsDeleted
);
