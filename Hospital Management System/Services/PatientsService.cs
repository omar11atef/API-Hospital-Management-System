namespace Hospital_Management_System.Services;

public class PatientsService(ApplicationDbContext context) : IPatientsServices
{
    private readonly ApplicationDbContext _context = context;
    public async Task<IEnumerable<Patients>> GetAllPatientsAsync(CancellationToken cancellationToken)
    {
        return await _context.Patients
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<Patients>> GetAllPatientDeletedAsync(CancellationToken cancellationToken)
    {
        return await _context.Patients
            .Where(p => p.IsDeleted)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<Patients>> GetAllPatientNotDeletedAsync(CancellationToken cancellationToken)
    {
        return await _context.Patients
            //.Where(p => !p.IsDeleted)
            .Where(p => p.IsDeleted == false)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Patients?> GetPatientByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Patients.FindAsync(id, cancellationToken);
    }
    public async Task<Patients?> CreatePatientAsync(Patients patient, CancellationToken cancellationToken = default)
    {
        if (patient is null)
            return null;
        var exists = await _context.Patients
                            .AnyAsync(x => x.NationalId == patient.NationalId, cancellationToken);
        if (exists) return null;

        await _context.AddAsync(patient, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return patient;
    }
    public async Task<bool> UpdatePatientAsync(int id, Patients patient, CancellationToken cancellationToken = default)
    {
        var existingPatient = await _context.Patients.FindAsync(id, cancellationToken);
        if (existingPatient is null)
            return false;
        existingPatient.Name = patient.Name;
        existingPatient.DateOfBirth = patient.DateOfBirth;
        existingPatient.Address = patient.Address;
        existingPatient.PhoneNumber = patient.PhoneNumber;
        existingPatient.NationalId = patient.NationalId;
        existingPatient.Gender = patient.Gender;
        existingPatient.DiseaseName = patient.DiseaseName;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<bool> DeletePatientAsync(int id, CancellationToken cancellationToken = default)
    {
        var existingPatient = await _context.Patients.FindAsync(id, cancellationToken);
        if (existingPatient is null)
            return false;
        existingPatient.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<bool> TogglePatientExiteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existingPatient = await _context.Patients.FindAsync(id, cancellationToken);
        if (existingPatient is null)
            return false;
        existingPatient.IsDeleted = !existingPatient.IsDeleted;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(bool IsSuccess, string Message, decimal? NewAmount)> UpdateMaxMedicalExpensesAsync(
    int id,
    UpdateExpensesRequest newMaxMedicalExpenses,
    CancellationToken cancellationToken = default)
    {
        var existingPatient = await _context.Patients.FindAsync(new object[] { id }, cancellationToken);

        if (existingPatient is null)
        {
            return (false, "Error: Patient not found.", null);
        }

        if (newMaxMedicalExpenses.Amount <= 0)
        {
            return (false, "Error: Amount must be a positive number greater than zero.", null);
        }
        //(Apply Update)
        existingPatient.MaxMedicalExpenses = newMaxMedicalExpenses.Amount;

        await _context.SaveChangesAsync(cancellationToken);
        // return success message with the new amount
        return (true, "Success: Max Medical Expenses updated successfully.", existingPatient.MaxMedicalExpenses);
    }
}
