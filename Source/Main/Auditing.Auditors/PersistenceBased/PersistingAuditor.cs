namespace Auditing.Auditors.PersistenceBased;

public class PersistingAuditor : IHttpRequestAuditor
{
    private readonly IAuditRepository _repository;

    public PersistingAuditor(IAuditRepository repository)
    {
        _repository = repository;
    }

    public void AuditPost(Guid entityId)
    {
        Audit(entityId, HttpRequestType.Post);
    }

    public void AuditDelete(Guid entityId)
    {
        Audit(entityId, HttpRequestType.Delete);
    }

    private void Audit(Guid entityId, HttpRequestType httpRequestType)
    {
        _repository.CreateEntry(entityId, httpRequestType);
    }
}
