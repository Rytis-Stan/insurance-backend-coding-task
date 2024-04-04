namespace Claims.Auditing;

public class PersistingCoverAuditor : PersistingAuditor<ICoverAuditRepository>, ICoverAuditor
{
    public PersistingCoverAuditor(ICoverAuditRepository repository)
        : base(repository)
    {
    }
}