﻿using Auditing.Auditors.PersistenceBased;

namespace Auditing.Persistence;

public interface IAuditDatabase : IDisposable
{
    ICoverAuditRepository CoverAuditRepository { get; }
    IClaimAuditRepository ClaimAuditRepository { get; }
    void Migrate();
}
