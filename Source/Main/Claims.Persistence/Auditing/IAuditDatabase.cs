using Claims.Auditing;

namespace Claims.Persistence.Auditing;

public interface IAuditDatabase : IDisposable
{
    ICoverAuditRepository CoverAuditRepository { get; }
    IClaimAuditRepository ClaimAuditRepository { get; }
    void Migrate();
}
