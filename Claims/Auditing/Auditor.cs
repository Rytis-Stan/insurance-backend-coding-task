﻿using Claims.Infrastructure;

namespace Claims.Auditing;

public class Auditor : IClaimAuditor, ICoverAuditor
{
    private readonly AuditContext _auditContext;
    private readonly IClock _clock;

    public Auditor(AuditContext auditContext, IClock clock)
    {
        _auditContext = auditContext;
        _clock = clock;
    }

    public void AuditClaim(Guid id, string httpRequestType)
    {
        var claimAudit = new ClaimAudit
        {
            Created = _clock.UtcNow(),
            HttpRequestType = httpRequestType,
            ClaimId = id.ToString()
        };

        _auditContext.Add(claimAudit);
        _auditContext.SaveChanges();
    }
        
    public void AuditCover(Guid id, string httpRequestType)
    {
        var coverAudit = new CoverAudit
        {
            Created = _clock.UtcNow(),
            HttpRequestType = httpRequestType,
            CoverId = id.ToString()
        };

        _auditContext.Add(coverAudit);
        _auditContext.SaveChanges();
    }
}

public class RabbitMqAuditor : IClaimAuditor, ICoverAuditor
{
    public void AuditClaim(Guid id, string httpRequestType)
    {
        throw new NotImplementedException();
    }

    public void AuditCover(Guid id, string httpRequestType)
    {
        throw new NotImplementedException();
    }
}
