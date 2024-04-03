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

    public void AuditClaimPost(Guid id)
    {
        AuditClaim(id, HttpRequestType.Post);
    }

    public void AuditClaimDelete(Guid id)
    {
        AuditClaim(id, HttpRequestType.Delete);
    }

    private void AuditClaim(Guid id, HttpRequestType httpRequestType)
    {
        var claimAudit = new ClaimAudit
        {
            Created = _clock.UtcNow(),
            HttpRequestType = httpRequestType.ToString(),
            ClaimId = id.ToString()
        };

        _auditContext.Add(claimAudit);
        _auditContext.SaveChanges();
    }

    public void AuditCoverPost(Guid id)
    {
        AuditCover(id, HttpRequestType.Post);
    }

    public void AuditCoverDelete(Guid id)
    {
        AuditCover(id, HttpRequestType.Delete);
    }

    private void AuditCover(Guid id, HttpRequestType httpRequestType)
    {
        var coverAudit = new CoverAudit
        {
            Created = _clock.UtcNow(),
            HttpRequestType = httpRequestType.ToString(),
            CoverId = id.ToString()
        };

        _auditContext.Add(coverAudit);
        _auditContext.SaveChanges();
    }
}
