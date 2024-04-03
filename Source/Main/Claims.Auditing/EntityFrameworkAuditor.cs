using Claims.Infrastructure;

namespace Claims.Auditing;

public abstract class EntityFrameworkAuditor<TAuditEntry> : IHttpRequestAuditor
{
    private readonly AuditContext _auditContext;
    private readonly IClock _clock;

    protected EntityFrameworkAuditor(AuditContext auditContext, IClock clock)
    {
        _auditContext = auditContext;
        _clock = clock;
    }

    public void AuditPost(Guid entityId)
    {
        Audit(entityId, HttpRequestType.Post);
    }

    public void AuditDelete(Guid entityId)
    {
        Audit(entityId, HttpRequestType.Delete);
    }

    private void Audit(Guid entityId, HttpRequestType httpRequestType)
    {
        var auditEntry = CreateAuditEntry(entityId, httpRequestType, _clock.UtcNow());
        _auditContext.Add(auditEntry!);
        _auditContext.SaveChanges();
    }

    protected abstract TAuditEntry CreateAuditEntry(Guid entityId, HttpRequestType httpRequestType, DateTime created);
}