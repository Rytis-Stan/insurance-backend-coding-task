namespace Claims.Auditing.PersistenceBased;

public class PersistingCoverAuditor : PersistingAuditor<ICoverAuditRepository>, ICoverAuditor
{
    public PersistingCoverAuditor(ICoverAuditRepository repository)
        : base(repository)
    {
    }
}