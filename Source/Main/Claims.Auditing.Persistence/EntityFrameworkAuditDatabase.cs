using BuildingBlocks.Temporal;
using Claims.Auditing.PersistenceBased;
using Microsoft.EntityFrameworkCore;

namespace Claims.Auditing.Persistence;

public class EntityFrameworkAuditDatabase : IAuditDatabase
{
    private readonly AuditContext _auditContext;
    private readonly IClock _clock;

    public ICoverAuditRepository CoverAuditRepository => new EntityFrameworkCoverAuditRepository(_auditContext, _clock);
    public IClaimAuditRepository ClaimAuditRepository => new EntityFrameworkClaimAuditRepository(_auditContext, _clock);

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
