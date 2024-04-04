namespace Claims.Auditing;

public class RepositoryBasedClaimAuditor : RepositoryBasedAuditor<IClaimAuditRepository>, IClaimAuditor
{
    public RepositoryBasedClaimAuditor(IClaimAuditRepository repository)
        : base(repository)
    {
    }
}