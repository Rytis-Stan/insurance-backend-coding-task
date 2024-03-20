using Claims.Infrastructure;

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

    public void AuditClaim(string id, string httpRequestType)
    {
        var claimAudit = new ClaimAudit
        {
            Created = _clock.Now(),
            HttpRequestType = httpRequestType,
            ClaimId = id
        };

        _auditContext.Add(claimAudit);
        _auditContext.SaveChanges();
    }
        
    public void AuditCover(string id, string httpRequestType)
    {
        var coverAudit = new CoverAudit
        {
            Created = _clock.Now(),
            HttpRequestType = httpRequestType,
            CoverId = id
        };

        _auditContext.Add(coverAudit);
        _auditContext.SaveChanges();
    }
}
