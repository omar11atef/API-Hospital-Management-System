namespace Hospital_Management_System.Contracts.Requests;

public record RequestDoctor
(
    string Name,
    string Specialization ,
    string Address,
    DateOnly? DateOfBirth,
    string AcademicDegree,
    string PhoneNumber ,
    string NationalId ,
    DateOnly? CreatedAt 
);
