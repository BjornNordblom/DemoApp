namespace DemoApp.Persistence;

using DemoApp.Entities;
using DemoApp.Services;
using Microsoft.EntityFrameworkCore;

//https://github.com/Ngineer101/auditing-dotnet-entity-framework-core
public class AppDbContext : DbContext
{
    protected readonly IConfiguration _config;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IDateTimeService _dateTimeService;
    private readonly IHttpContextAccessor _httpContext;
    private readonly string _username;

    public AppDbContext(
        IConfiguration configuration,
        ILoggerFactory loggerFactory,
        IDateTimeService dateTimeService,
        IHttpContextAccessor httpContext
    )
    {
        _config = configuration;
        _loggerFactory = loggerFactory;
        _dateTimeService = dateTimeService;
        _httpContext = httpContext;
        var claimsPrincipal = _httpContext.HttpContext?.User;
        _username =
            claimsPrincipal?.Claims?.SingleOrDefault(c => c.Type == "username")?.Value
            ?? "Unauthenticated user";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to sqlite database
        options.UseSqlite(_config.GetConnectionString("Default"));
        options.UseLoggerFactory(_loggerFactory);
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default
    )
    {
        // Get audit entries
        var auditEntries = OnBeforeSaveChanges();

        foreach (
            var e in this.ChangeTracker
                .Entries()
                .Where(e => e.Entity is IAuditable && e.State == EntityState.Added)
                .Select(e => e.Entity as IAuditable)
        )
        {
            e.CreatedAt = _dateTimeService.UtcNow;
        }
        foreach (
            var e in this.ChangeTracker
                .Entries()
                .Where(e => e.Entity is IAuditable && e.State == EntityState.Modified)
                .Select(e => e.Entity as IAuditable)
        )
        {
            e.UpdatedAt = _dateTimeService.UtcNow;
        }
        foreach (
            IAuditable e in this.ChangeTracker
                .Entries()
                .Where(e => e.Entity is IAuditable && e.State == EntityState.Deleted)
                .Select(e => e.Entity as IAuditable)
        )
        {
            e.DeletedAt = _dateTimeService.UtcNow;
        }

        // Save current entity
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        // Save audit entries
        await OnAfterSaveChangesAsync(auditEntries);
        return result;
    }

    private List<AuditTrace> OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();
        var entries = new List<AuditTrace>();

        foreach (var entry in ChangeTracker.Entries())
        {
            // Dot not audit entities that are not tracked, not changed, or not of type IAuditable
            if (
                entry.State == EntityState.Detached
                || entry.State == EntityState.Unchanged
                || !(entry.Entity is IAuditable)
            )
                continue;

            var AuditTrace = new AuditTrace
            {
                ActionType =
                    entry.State == EntityState.Added
                        ? "INSERT"
                        : entry.State == EntityState.Deleted
                            ? "DELETE"
                            : "UPDATE",
                EntityId = entry.Properties
                    .Single(p => p.Metadata.IsPrimaryKey())
                    .CurrentValue.ToString(),
                EntityName = entry.Metadata.ClrType.Name,
                Username = _username,
                TimeStamp = _dateTimeService.UtcNow,
                Changes = entry.Properties
                    .Select(p => new { p.Metadata.Name, p.CurrentValue })
                    .ToDictionary(i => i.Name, i => i.CurrentValue),
                // TempProperties are properties that are only generated on save, e.g. ID's
                // These properties will be set correctly after the audited entity has been saved
                TempProperties = entry.Properties.Where(p => p.IsTemporary).ToList(),
            };

            entries.Add(AuditTrace);
        }

        return entries;
    }

    private Task OnAfterSaveChangesAsync(List<AuditTrace> auditEntries)
    {
        if (auditEntries == null || auditEntries.Count == 0)
            return Task.CompletedTask;

        // For each temporary property in each audit entry - update the value in the audit entry to the actual (generated) value
        foreach (var entry in auditEntries)
        {
            foreach (var prop in entry.TempProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                {
                    entry.EntityId = prop.CurrentValue.ToString();
                    entry.Changes[prop.Metadata.Name] = prop.CurrentValue;
                }
                else
                {
                    entry.Changes[prop.Metadata.Name] = prop.CurrentValue;
                }
            }
        }

        AuditTraces.AddRange(auditEntries);
        return SaveChangesAsync();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<UserPost> UserPosts { get; set; }
    protected DbSet<AuditTrace> AuditTraces { get; set; }
}
