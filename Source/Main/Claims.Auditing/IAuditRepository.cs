namespace Claims.Auditing;

public interface IAuditRepository
{
    void CreateEntry(Guid entityId, HttpRequestType httpRequestType);
}