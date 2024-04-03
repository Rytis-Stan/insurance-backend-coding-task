namespace Claims.Auditing;

public interface IHttpRequestAuditor
{
    void AuditPost(Guid entityId);
    void AuditDelete(Guid entityId);
}