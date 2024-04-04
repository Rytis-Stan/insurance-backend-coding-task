using Claims.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Claims.Auditing;

public interface IAuditDatabase : IDisposable
{
    void Migrate();
    ICoverAuditRepository CoverAuditRepository { get; }
    IClaimAuditRepository ClaimAuditRepository { get; }
}

public class EntityFrameworkAuditDatabase : IAuditDatabase
{
    private readonly AuditContext _auditContext;
    private readonly IClock _clock;

    public ICoverAuditRepository CoverAuditRepository => new CoverAuditRepository(_auditContext, _clock);
    public IClaimAuditRepository ClaimAuditRepository => new ClaimAuditRepository(_auditContext, _clock);

    public EntityFrameworkAuditDatabase(string connectionString)
        : this(CreateAuditContext(connectionString), new Clock())
    {
    }

    private EntityFrameworkAuditDatabase(AuditContext auditContext, IClock clock)
    {
        _auditContext = auditContext;
        _clock = clock;
    }

    public void Dispose()
    {
        _auditContext.Dispose();
    }

    public void Migrate()
    {
        _auditContext.Database.Migrate();
    }

    public static AuditContext CreateAuditContext(string connectionString)
    {
        return new AuditContext(DbContextOptions(connectionString));
    }

    private static DbContextOptions<AuditContext> DbContextOptions(string connectionString)
    {
        return new DbContextOptionsBuilder<AuditContext>().UseSqlServer(connectionString).Options;
    }
}
