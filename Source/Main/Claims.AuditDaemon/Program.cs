using Claims.Auditing;
using Claims.Auditing.MessageQueues.RabbitMq;
using Claims.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Claims.AuditDaemon;

public class Program
{
    static void Main()
    {
        // TODO: Move the queue name (and some options???) to the configuration file!
        using var messageQueues = new InactiveRabbitMqReceivingQueue<AuditMessage>("localhost", "Claims.AuditQueue").Activate();

        Console.WriteLine("Starting to listed to messages.");

        var connectionString = "Server=localhost\\SQLEXPRESS;Database=AuditDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
        var dbContextOptions = new DbContextOptionsBuilder<AuditContext>().UseSqlServer(connectionString).Options;

        var auditContext = new AuditContext(dbContextOptions);
        var clock = new Clock();
        var switchingAuditor = new SwitchingAuditor(auditContext, clock);
        messageQueues.OnReceived(message =>
        {
            Console.WriteLine($"RECEIVED_AT: {DateTime.UtcNow}, MESSAGE: {message}");
            switchingAuditor.OnMessageReceived(message);
        });

        Console.WriteLine("Press [enter] to quit.");
        Console.ReadLine();
    }

    private class SwitchingAuditor
    {
        private readonly Dictionary<AuditEntityKind, IHttpRequestAuditor> _auditorsByAuditEntityKind;

        public SwitchingAuditor(AuditContext auditContext, IClock clock)
            : this(new Dictionary<AuditEntityKind, IHttpRequestAuditor>
            {
                { AuditEntityKind.Cover, new EntityFrameworkCoverAuditor(auditContext, clock) },
                { AuditEntityKind.Claim, new EntityFrameworkClaimAuditor(auditContext, clock) }
            })
        {
        }

        public SwitchingAuditor(Dictionary<AuditEntityKind, IHttpRequestAuditor> auditorsByAuditEntityKind)
        {
            _auditorsByAuditEntityKind = auditorsByAuditEntityKind;
        }

        public void OnMessageReceived(AuditMessage message)
        {
            var auditor = _auditorsByAuditEntityKind[message.EntityType];
            switch (message.HttpRequestType)
            {
                case HttpRequestType.Post:
                    auditor.AuditPost(message.EntityId);
                    break;
                case HttpRequestType.Delete:
                    auditor.AuditDelete(message.EntityId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
