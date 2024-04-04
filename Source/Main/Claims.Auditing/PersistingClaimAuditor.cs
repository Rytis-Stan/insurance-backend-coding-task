namespace Claims.Auditing;

public class PersistingClaimAuditor : PersistingAuditor<IClaimAuditRepository>, IClaimAuditor
{
    public PersistingClaimAuditor(IClaimAuditRepository repository)
        : base(repository)
    {
    }
}