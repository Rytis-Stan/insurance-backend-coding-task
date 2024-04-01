namespace Claims.Auditing;

public interface ICoverAuditor
{
    void AuditCoverPost(Guid id);
    void AuditCoverDelete(Guid id);
}
