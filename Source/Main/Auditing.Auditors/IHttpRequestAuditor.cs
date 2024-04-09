namespace Auditing.Auditors;

public interface IHttpRequestAuditor
{
    void AuditPost(Guid entityId);
    void AuditDelete(Guid entityId);
}