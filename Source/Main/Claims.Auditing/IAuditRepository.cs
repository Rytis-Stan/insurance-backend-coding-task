using Claims.Infrastructure;

namespace Claims.Auditing;

public interface IAuditRepository
{
    void CreateEntry(Guid entityId, HttpRequestType httpRequestType);
}

public interface IClaimAuditRepository : IAuditRepository
{
}

public interface ICoverAuditRepository : IAuditRepository
{
}

public abstract class AuditRepository<TAuditEntry> : IAuditRepository
{
    private readonly AuditContext _auditContext;
    private readonly IClock _clock;

    protected AuditRepository(AuditContext auditContext, IClock clock)
    {
        _auditContext = auditContext;
        _clock = clock;
    }

    public void CreateEntry(Guid entityId, HttpRequestType httpRequestType)
    {
        var auditEntry = CreateAuditEntry(entityId, httpRequestType, _clock.UtcNow());
        _auditContext.Add(auditEntry!);
        _auditContext.SaveChanges();
    }

    protected abstract TAuditEntry CreateAuditEntry(Guid entityId, HttpRequestType httpRequestType, DateTime created);
}

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

internal class ClaimAuditRepository : AuditRepository<ClaimAudit>, IClaimAuditRepository
{
    public ClaimAuditRepository(AuditContext auditContext, IClock clock)
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