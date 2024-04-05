using BuildingBlocks.Temporal;
using Claims.Auditing;
using Claims.Auditing.PersistenceBased;

namespace Claims.Persistence.Auditing;

public abstract class AuditRepository<TAuditEntry> : IAuditRepository
{
    private readonly AuditContext _auditContext;
    private readonly IClock _clock;

    protected AuditRepository(AuditContext auditContext, IClock clock)
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