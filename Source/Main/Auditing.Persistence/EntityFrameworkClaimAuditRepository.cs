using Auditing.Auditors;
using BuildingBlocks.Temporal;

namespace Auditing.Persistence;

internal class EntityFrameworkClaimAuditRepository : EntityFrameworkAuditRepository<ClaimAudit>
{
    public EntityFrameworkClaimAuditRepository(AuditContext auditContext, IClock clock)
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