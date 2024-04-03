using Claims.Infrastructure;

namespace Claims.Auditing;

public class CoverAuditor : HttpRequestAuditor<CoverAudit>, ICoverAuditor
{
    public CoverAuditor(AuditContext auditContext, IClock clock)
        : base(auditContext, clock)
    {
    }

    protected override CoverAudit CreateAuditEntry(Guid entityId, HttpRequestType httpRequestType, DateTime created)
    {
        return new CoverAudit
        {
            CoverId = entityId.ToString(),
            HttpRequestType = httpRequestType.ToString(),
            Created = created,
        };
    }
}