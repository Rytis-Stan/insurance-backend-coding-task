namespace Auditing.Auditors.PersistenceBased;

public class PersistingCoverAuditor : PersistingAuditor<ICoverAuditRepository>, ICoverAuditor
{
    public PersistingCoverAuditor(ICoverAuditRepository repository)
        : base(repository)
    {
    }
}