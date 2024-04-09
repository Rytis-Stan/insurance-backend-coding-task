using Auditing.Auditors;
using Auditing.Auditors.PersistenceBased;
using BuildingBlocks.Temporal;

namespace Auditing.Persistence;

public abstract class EntityFrameworkAuditRepository<TAuditEntry> : IAuditRepository
{
    private readonly AuditContext _auditContext;
    private readonly IClock _clock;

    protected EntityFrameworkAuditRepository(AuditContext auditContext, IClock clock)
    {
        _auditContext = auditContext;
        _clock = clock;
    }

    public void CreateEntry(Guid entityId, HttpRequestType httpRequestType)
    {
        var auditEntry = CreateAuditEntry(entityId, httpRequestType, _clock.UtcNow());
        _auditContext.Add(auditEntry!);
        _auditContext.SaveChanges();
    }

    protected abstract TAuditEntry CreateAuditEntry(Guid entityId, HttpRequestType httpRequestType, DateTime created);
}