using Claims.AuditDaemon.Configuration;
using Claims.Auditing;
using Claims.Auditing.MessageQueues;
using Claims.Auditing.MessageQueues.RabbitMq;
using Claims.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Claims.AuditDaemon;

public class App
{
    private readonly AppConfiguration _configuration;

    public App(AppConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Run()
    {
        Console.WriteLine("Starting to listed to messages.");

        StartListeningToMessages();
        
        Console.WriteLine("Press [enter] to quit.");
        Console.ReadLine();
    }

    private void StartListeningToMessages()
    {
        using var messageQueue = ConnectToQueue(_configuration.RabbitMq);
        using var auditContext = CreateAuditContext();
        var switchingAuditor = new SwitchingAuditor(auditContext, new Clock());
        messageQueue.OnReceived(message =>
        {
            Console.WriteLine($"RECEIVED_AT: {DateTime.UtcNow}, MESSAGE: {message}");
            switchingAuditor.OnMessageReceived(message);
        });
    }

    private static IReceivingQueue<AuditMessage> ConnectToQueue(RabbitMqConfiguration configuration)
    {
        return new InactiveRabbitMqReceivingQueue<AuditMessage>(configuration.HostName, configuration.QueueName).Activate();
    }

    private AuditContext CreateAuditContext()
    {
        var dbContextOptions = new DbContextOptionsBuilder<AuditContext>().UseSqlServer(_configuration.ConnectionString).Options;
        return new AuditContext(dbContextOptions);
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
