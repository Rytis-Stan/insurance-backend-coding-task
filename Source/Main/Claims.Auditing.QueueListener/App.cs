using BuildingBlocks.MessageQueues;
using BuildingBlocks.MessageQueues.RabbitMq;
using Claims.Auditing.MessageQueueBased;
using Claims.Auditing.PersistenceBased;
using Claims.Auditing.QueueListener.Configuration;
using Claims.Persistence.Auditing;

namespace Claims.Auditing.QueueListener;

public class App
{
    private readonly AppConfiguration _configuration;

    public App(AppConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Run()
    {
        Console.WriteLine("Starting to migrate the auditing database.");
        
        MigrateAuditDatabase(_configuration.ConnectionString);

        Console.WriteLine("Finishing migration of the auditing database.");
        Console.WriteLine("Starting to listed to messages.");

        StartListeningToAuditMessages();
        
        Console.WriteLine("Press [Enter] to quit.");
        Console.ReadLine();
    }

    // TODO: Ensure that the same audit database instance gets reused both for migration and for setting up the auditors.
    private static void MigrateAuditDatabase(string connectionString)
    {
        new EntityFrameworkAuditDatabase(connectionString).Migrate();
    }

    private void StartListeningToAuditMessages()
    {
        using var messageQueue = ConnectToQueue(_configuration.RabbitMq);
        using var auditDatabase = new EntityFrameworkAuditDatabase(_configuration.ConnectionString);
        var auditor = new AuditingQueueListener(auditDatabase);
        messageQueue.OnReceived(message =>
        {
            Console.WriteLine($"RECEIVED_AT: {DateTime.UtcNow}, MESSAGE: {message}");
            auditor.OnMessageReceived(message);
        });
    }

    private static IReceivingQueue<AuditMessage> ConnectToQueue(RabbitMqConfiguration configuration)
    {
        return new InactiveRabbitMqReceivingQueue<AuditMessage>(configuration.HostName, configuration.QueueName).Activate();
    }

    // TODO: Move this code to "Claims.Auditing" project?
    private class AuditingQueueListener
    {
        private readonly Dictionary<AuditEntityKind, IHttpRequestAuditor> _auditorsByAuditEntityKind;

        public AuditingQueueListener(IAuditDatabase auditDatabase)
            : this(new Dictionary<AuditEntityKind, IHttpRequestAuditor>
            {
                { AuditEntityKind.Cover, new PersistingCoverAuditor(auditDatabase.CoverAuditRepository) },
                { AuditEntityKind.Claim, new PersistingClaimAuditor(auditDatabase.ClaimAuditRepository) }
            })
        {
        }

        public AuditingQueueListener(Dictionary<AuditEntityKind, IHttpRequestAuditor> auditorsByAuditEntityKind)
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
