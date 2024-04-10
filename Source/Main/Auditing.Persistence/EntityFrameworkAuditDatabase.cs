using Auditing.Auditors.PersistenceBased;
using BuildingBlocks.Temporal;
using Microsoft.EntityFrameworkCore;

namespace Auditing.Persistence;

public class EntityFrameworkAuditDatabase : IAuditDatabase
{
    private readonly AuditContext _auditContext;
    private readonly IClock _clock;

    public IAuditRepository CoverAuditRepository => new EntityFrameworkCoverAuditRepository(_auditContext, _clock);
    public IAuditRepository ClaimAuditRepository => new EntityFrameworkClaimAuditRepository(_auditContext, _clock);

    public EntityFrameworkAuditDatabase(AuditContext auditContext, IClock clock)
    {
        _auditContext = auditContext;
        _clock = clock;
    }

    public void Dispose()
    {
        _auditContext.Dispose();
        GC.SuppressFinalize(this);
    }

    public void Migrate()
    {
        _auditContext.Database.Migrate();
    }
}
