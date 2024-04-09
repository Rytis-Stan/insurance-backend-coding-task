namespace Shared.Auditing.PersistenceBased;

public interface IAuditRepository
{
    void CreateEntry(Guid entityId, HttpRequestType httpRequestType);
}