using Auditing.Auditors.PersistenceBased;

namespace Auditing.Persistence;

public interface IAuditDatabase : IDisposable
{
    IAuditRepository CoverAuditRepository { get; }
    IAuditRepository ClaimAuditRepository { get; }
    void Migrate();
}
