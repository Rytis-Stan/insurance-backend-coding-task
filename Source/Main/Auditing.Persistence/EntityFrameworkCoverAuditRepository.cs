using Auditing.Auditors;
using Auditing.Auditors.PersistenceBased;
using BuildingBlocks.Temporal;

namespace Auditing.Persistence;

internal class EntityFrameworkCoverAuditRepository : EntityFrameworkAuditRepository<CoverAudit>, ICoverAuditRepository
{
    public EntityFrameworkCoverAuditRepository(AuditContext auditContext, IClock clock)
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