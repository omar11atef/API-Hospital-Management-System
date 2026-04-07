namespace Hospital_Management_System.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options , IHttpContextAccessor httpContextAccessor) :
    IdentityDbContext<ApplicationUser,ApplicationRoles,string>(options)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<PatientRoom> PatientRooms { get; set; }
    public DbSet<PatientVisit> PatientVisits { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Appointment> Appointments { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Select All FK where OnDelete is --> Cascade
        var CascadeFK = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(x => x.GetForeignKeys())
            .Where(f => f.DeleteBehavior == DeleteBehavior.Cascade && !f.IsOwnership);

        // Change Value --> Restrict
        foreach (var f in CascadeFK)
        {
            f.DeleteBehavior = DeleteBehavior.Restrict;
        }
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    /*public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Get UserId from Request :
        var currentUser = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        // Get Specific Entires from table
        var entries = ChangeTracker.Entries<AuditableEntity>();
        foreach (var entry in entries)
        {
            // Check Create == CreateForCurrentUser
            if(entry.State == EntityState.Added)
            {
                entry.Property(x=>x.CreatedById).CurrentValue = currentUser;
            }
            // Check Update == UpdateForCurrentUser ,Change Value Column UpdateOn
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.UpdatedById).CurrentValue = currentUser;
                entry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
            }
        }   
        return base.SaveChangesAsync(cancellationToken);
    }*/

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries<AuditableEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.CreatedBy = GetCurrentUserId();
            }

            entry.Entity.ModifiedAt = DateTime.UtcNow;
            entry.Entity.ModifiedBy = GetCurrentUserId();
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
    private string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
    }



}
