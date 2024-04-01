using Claims.Infrastructure;

namespace Claims.Api.Auditing;

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
        AuditClaim(id, "POST");
    }

    public void AuditClaimDelete(Guid id)
    {
        AuditClaim(id, "DELETE");
    }

    private void AuditClaim(Guid id, string httpRequestType)
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

    public void AuditCoverPost(Guid id)
    {
        AuditCover(id, "POST");
    }

    public void AuditCoverDelete(Guid id)
    {
        AuditCover(id, "DELETE");
    }

    private void AuditCover(Guid id, string httpRequestType)
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

// public class RabbitMqAuditor : IClaimAuditor, ICoverAuditor
// {
//     public void AuditClaim(Guid id, string httpRequestType)
//     {
//         throw new NotImplementedException();
//     }
//
//     public void AuditCover(Guid id, string httpRequestType)
//     {
//         throw new NotImplementedException();
//     }
// }
