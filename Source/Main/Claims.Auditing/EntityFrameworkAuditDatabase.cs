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
        var dbContextOptions = new DbContextOptionsBuilder<AuditContext>().UseSqlServer(connectionString).Options;
        return new AuditContext(dbContextOptions);
    }
}
