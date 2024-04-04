using Claims.Auditing;
using Claims.Infrastructure;

namespace Claims.Persistence.Auditing;

internal class CoverAuditRepository : AuditRepository<CoverAudit>, ICoverAuditRepository
{
    public CoverAuditRepository(AuditContext auditContext, IClock clock)
        : base(auditContext, clock)
    {
    }

    protected override CoverAudit CreateAuditEntry(Guid entityId, HttpRequestType httpRequestType, DateTime created)
    {
        return new CoverAudit
        {
            CoverId = entityId.ToString(),
            HttpRequestType = httpRequestType.ToString(),
            Created = created
        };
    }
}