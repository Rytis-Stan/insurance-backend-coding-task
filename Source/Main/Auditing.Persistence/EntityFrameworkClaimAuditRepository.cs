﻿using Auditing.Auditors;
using Auditing.Auditors.PersistenceBased;
using BuildingBlocks.Temporal;

namespace Auditing.Persistence;

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