namespace Claims.Auditing;

public class RepositoryBasedCoverAuditor : RepositoryBasedAuditor<ICoverAuditRepository>, ICoverAuditor
{
    public RepositoryBasedCoverAuditor(ICoverAuditRepository repository)
        : base(repository)
    {
    }
}