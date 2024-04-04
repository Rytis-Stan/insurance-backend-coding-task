namespace Claims.Auditing;

public interface IAuditDatabase : IDisposable
{
    void Migrate();
    ICoverAuditRepository CoverAuditRepository { get; }
    IClaimAuditRepository ClaimAuditRepository { get; }
}
