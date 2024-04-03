using Claims.Infrastructure;

namespace Claims.Auditing;

public class ClaimAuditor : HttpRequestAuditor<ClaimAudit>, IClaimAuditor
{
    public ClaimAuditor(AuditContext auditContext, IClock clock)
        : base(auditContext, clock)
    {
    }

    protected override ClaimAudit CreateAuditEntry(Guid entityId, HttpRequestType httpRequestType, DateTime created)
    {
        return new ClaimAudit
        {
            ClaimId = entityId.ToString(),
            HttpRequestType = httpRequestType.ToString(),
            Created = created
        };
    }
}