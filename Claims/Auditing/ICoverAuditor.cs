namespace Claims.Auditing;

public interface ICoverAuditor
{
    void AuditCover(string id, string httpRequestType);
}
