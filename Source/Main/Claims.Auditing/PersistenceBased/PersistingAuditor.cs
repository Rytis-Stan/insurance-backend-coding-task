namespace Claims.Auditing.PersistenceBased;

public abstract class PersistingAuditor<TRepository> : IHttpRequestAuditor
    where TRepository : IAuditRepository
{
    private readonly TRepository _repository;

    protected PersistingAuditor(TRepository repository)
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