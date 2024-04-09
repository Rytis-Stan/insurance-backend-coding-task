namespace Auditing.Auditors.PersistenceBased;

public interface IAuditRepository
{
    void CreateEntry(Guid entityId, HttpRequestType httpRequestType);
}