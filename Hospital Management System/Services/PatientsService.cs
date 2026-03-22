using Hospital_Management_System.Entities;
using System.Numerics;

namespace Hospital_Management_System.Services;

public class PatientsService(ApplicationDbContext context) : IPatientsServices
{
    private readonly ApplicationDbContext _context = context;
    public async Task<Result<IEnumerable<Patient>>> GetAllPatientsAsync(CancellationToken cancellationToken)
    {
        var result = await _context.Patients
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return Result.Success<IEnumerable<Patient>>(result);
    }
    public async Task<Result<IEnumerable<Patient>>> GetAllPatientDeletedAsync(CancellationToken cancellationToken)
    {
        var result= await _context.Patients
            .Where(p => p.IsDeleted)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return Result.Success<IEnumerable<Patient>>(result);
    }
    public async Task<Result<IEnumerable<Patient>>> GetAllPatientNotDeletedAsync(CancellationToken cancellationToken)
    {
        var result = await _context.Patients
            .Where(p => p.IsDeleted == false)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return Result.Success<IEnumerable<Patient>>(result);
    }
    public async Task<Result<ResponePatient>> GetPatientByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _context.Patients.FindAsync(id, cancellationToken);
        return result is not null
           ? Result.Success(result.Adapt<ResponePatient>())
           : Result.Failure<ResponePatient>(PatientErrors.PatientNotFound);
    }
    public async Task<Result<ResponePatient>> CreatePatientAsync(int departmentId, RequestPatient patient, CancellationToken cancellationToken = default)
    {
        var departmentExists = await _context.Departments
              .AnyAsync(d => d.Id == departmentId, cancellationToken);
        if (!departmentExists)
            return Result.Failure<ResponePatient>(PatientErrors.DepartmentNotFound);

        var exists = await _context.Patients
                            .AnyAsync(x => x.NationalId == patient.NationalId, cancellationToken);
        if (exists) return Result.Failure<ResponePatient>(PatientErrors.DuplicateNationalId);

        var newpatient = patient.Adapt<Patient>();
        newpatient.DepartmentId = departmentId;
        await _context.AddAsync(newpatient, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var result = newpatient.Adapt<ResponePatient>();
        return Result.Success(result);
    }
    public async Task<Result<ResponePatient>> UpdatePatientAsync(int departmentId,int id, RequestPatient patient, CancellationToken cancellationToken = default)
    {
        var departmentExists = await _context.Departments
               .AnyAsync(d => d.Id == departmentId, cancellationToken);
        if (!departmentExists)
            return Result.Failure<ResponePatient>(PatientErrors.DepartmentNotFound);

        var existingPatient = await _context.Patients.FindAsync(id, cancellationToken);
        if (existingPatient is null)
            return Result.Failure<ResponePatient>(PatientErrors.PatientNotFound); 

        bool isDuplicateNationalId = await _context.Patients
            .AnyAsync(d =>d.NationalId == patient.NationalId && d.Id != id, cancellationToken);
        if (isDuplicateNationalId)
            return Result.Failure<ResponePatient>(PatientErrors.DuplicateNationalId);

        existingPatient.DepartmentId = departmentId;
        existingPatient.Name = patient.Name;
        existingPatient.DateOfBirth = patient.DateOfBirth;
        existingPatient.Address = patient.Address;
        existingPatient.PhoneNumber = patient.PhoneNumber;
        existingPatient.NationalId = patient.NationalId;
        existingPatient.Gender = patient.Gender;
        existingPatient.DiseaseName = patient.DiseaseName;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(existingPatient.Adapt<ResponePatient>());
    }
    public async Task<Result> DeletePatientAsync(int id, CancellationToken cancellationToken = default)
    {
        var existingPatient = await _context.Patients.FindAsync(id, cancellationToken);
        if (existingPatient is null)
            return Result.Failure(PatientErrors.PatientNotFound);
        existingPatient.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result> TogglePatientExiteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existingPatient = await _context.Patients.FindAsync(id, cancellationToken);
        if (existingPatient is null)
            return Result.Failure(PatientErrors.PatientNotFound); 
        existingPatient.IsDeleted = !existingPatient.IsDeleted;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(); 
    }

    public async Task<Result<decimal>> UpdateMaxMedicalExpensesAsync(int id, UpdateExpensesRequest request, CancellationToken cancellationToken = default)
    {
        var existingPatient = await _context.Patients.FindAsync(id, cancellationToken);
        if (existingPatient is null)
            return Result.Failure<decimal>(PatientErrors.PatientNotFound);

        if (request.Amount <= 0)
            return Result.Failure<decimal>(PatientErrors.InvalidAmount);

        existingPatient.MaxMedicalExpenses = request.Amount;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(existingPatient.MaxMedicalExpenses);
    }
}
