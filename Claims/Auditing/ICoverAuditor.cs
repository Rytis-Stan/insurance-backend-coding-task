namespace Claims.Auditing;

public interface ICoverAuditor
{
    void AuditCover(Guid id, string httpRequestType);
}
