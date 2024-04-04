using Microsoft.EntityFrameworkCore;

namespace Claims.Auditing;

public class EntityFrameworkAuditDatabase
{
    private readonly string _connectionString;

    public EntityFrameworkAuditDatabase(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Migrate()
    {
        var context = CreateAuditContext(_connectionString);
        context.Database.Migrate();
    }

    public static AuditContext CreateAuditContext(string connectionString)
    {
        return new AuditContext(DbContextOptions(connectionString));
    }

    private static DbContextOptions<AuditContext> DbContextOptions(string connectionString)
    {
        return new DbContextOptionsBuilder<AuditContext>().UseSqlServer(connectionString).Options;
    }
}
