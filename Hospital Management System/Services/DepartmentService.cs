using Hospital_Management_System.Contracts.DepartmentRequest;

namespace Hospital_Management_System.Services;

public class DepartmentService(ApplicationDbContext context) : IDepartmentService
{
    private readonly ApplicationDbContext _context =context;
    public async Task<Result<IEnumerable<Department>>> GetAllDepartmentsAsync(CancellationToken cancellationToken)
    {
        var result = await _context.Departments
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<Department>>(result);
    }

    public async Task<Result<DepartmentResponse>> GetDepartmentByIdAsync(int id, CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (department is null)
            return Result.Failure<DepartmentResponse>(DepartmentErrors.NotFound);

        return Result.Success(department.Adapt<DepartmentResponse>());
    }
    public async Task<Result<DepartmentResponse>> CreateDepartmentAsync(DepartmentRequest request, CancellationToken cancellationToken)
    {
        var nameExists = await _context.Departments
            .AnyAsync(d => d.Name == request.Name, cancellationToken);

        if (nameExists)
            return Result.Failure<DepartmentResponse>(DepartmentErrors.DuplicateName);

        var department = request.Adapt<Department>();
        await _context.Departments.AddAsync(department, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(department.Adapt<DepartmentResponse>());
    }
    public async Task<Result<DepartmentResponse>> UpdateDepartmentAsync(int id, DepartmentRequest request, CancellationToken cancellationToken)
    {
        var department = await _context.Departments.FindAsync([id], cancellationToken);

        if (department is null)
            return Result.Failure<DepartmentResponse>(DepartmentErrors.NotFound);

        var nameExists = await _context.Departments
            .AnyAsync(d => d.Name == request.Name && d.Id != id, cancellationToken);

        if (nameExists)
            return Result.Failure<DepartmentResponse>(DepartmentErrors.DuplicateName);

        department.Name = request.Name;
        department.Location = request.Location;
        department.PhoneNumber = request.PhoneNumber;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(department.Adapt<DepartmentResponse>());
    }
    /*public async Task<Result> DeleteDepartmentAsync(int id, CancellationToken cancellationToken)
    {
        var department = await _context.Departments.FindAsync([id], cancellationToken);

        if (department is null)
            return Result.Failure(DepartmentErrors.NotFound);
         department.IsDeleted = true; 
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }*/

    
    /*public async Task<Result> ToggleDepartmentExiteAsync(int id, CancellationToken cancellationToken)
    {
        var department = await _context.Departments.FindAsync([id], cancellationToken);

        if (department is null)
            return Result.Failure(DepartmentErrors.NotFound);
         department.IsDeleted = !department.IsDeleted;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }*/

}
