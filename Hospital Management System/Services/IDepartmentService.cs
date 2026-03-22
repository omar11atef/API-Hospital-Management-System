using Hospital_Management_System.Contracts.DepartmentRequest;

namespace Hospital_Management_System.Services;

public interface IDepartmentService
{
    //Task<Result<IEnumerable<DepartmentResponse>>> GetAllDepartmentsAsync(CancellationToken cancellationToken);
    Task<Result<IEnumerable<Department>>> GetAllDepartmentsAsync(CancellationToken cancellationToken);
    Task<Result<DepartmentResponse>> GetDepartmentByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<DepartmentResponse>> CreateDepartmentAsync(DepartmentRequest result, CancellationToken cancellationToken);
    Task<Result<DepartmentResponse>> UpdateDepartmentAsync( int id, DepartmentRequest patients, CancellationToken cancellationToken);
    //Task<Result> DeleteDepartmentAsync(int id, CancellationToken cancellationToken);
    //Task<Result> ToggleDepartmentExiteAsync(int id, CancellationToken cancellationToken);




}
