using BuildingBlocks.Temporal;
using Claims.Auditing;
using Claims.Auditing.PersistenceBased;

namespace Claims.Persistence.Auditing;

internal class EntityFrameworkClaimAuditRepository : EntityFrameworkAuditRepository<ClaimAudit>, IClaimAuditRepository
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