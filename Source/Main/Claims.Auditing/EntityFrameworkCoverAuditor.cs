using Claims.Infrastructure;

namespace Claims.Auditing;

public class EntityFrameworkCoverAuditor : EntityFrameworkAuditor<CoverAudit>, ICoverAuditor
{
    public EntityFrameworkCoverAuditor(AuditContext auditContext, IClock clock)
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