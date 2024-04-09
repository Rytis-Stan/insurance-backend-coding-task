using Microsoft.EntityFrameworkCore;

namespace Claims.Auditing.Persistence;

public class AuditContext : DbContext
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AuditContext(DbContextOptions<AuditContext> options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        : base(options)
    {
    }

    public DbSet<ClaimAudit> ClaimAudits { get; init; }
    public DbSet<CoverAudit> CoverAudits { get; init; }
}
