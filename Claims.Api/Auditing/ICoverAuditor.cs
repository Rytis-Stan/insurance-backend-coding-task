namespace Claims.Api.Auditing;

public interface ICoverAuditor
{
    void AuditCoverPost(Guid id);
    void AuditCoverDelete(Guid id);
}
