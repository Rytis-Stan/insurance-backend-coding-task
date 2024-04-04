namespace Claims.Auditing;

public abstract class RepositoryBasedAuditor<TRepository> : IHttpRequestAuditor
    where TRepository : IAuditRepository
{
    private readonly TRepository _repository;

    protected RepositoryBasedAuditor(TRepository repository)
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