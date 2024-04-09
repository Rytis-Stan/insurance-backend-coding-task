namespace Shared.Auditing;

public interface IHttpRequestAuditor
{
    void AuditPost(Guid entityId);
    void AuditDelete(Guid entityId);
}