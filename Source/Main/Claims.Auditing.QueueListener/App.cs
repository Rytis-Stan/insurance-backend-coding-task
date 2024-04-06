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

    // TODO: Return the disposables?
    private void StartListeningToAuditMessages()
    {
        using var auditDatabase = new EntityFrameworkAuditDatabase(_configuration.ConnectionString);
        using var messageQueue = ConnectToQueue(
            _configuration.RabbitMq, 
            new CompositeQueueListener<AuditMessage>(
                new LoggingQueueListener<AuditMessage>(),
                new AuditingQueueListener(auditDatabase)
            )
        );
        messageQueue.StartListening();
    }

    private static IConnectedReceivingQueue ConnectToQueue(RabbitMqConfiguration configuration, IQueueListener<AuditMessage> listner)
    {
        return new RabbitMqReceivingQueue<AuditMessage>(configuration.HostName, configuration.QueueName).Connect(listner);
    }

    private class LoggingQueueListener<TMessage> : IQueueListener<TMessage>
    {
        public void OnMessageReceived(TMessage message)
        {
            // TODO: Make the console writer an injectable dependency or change this into an actual log instance.
            Console.WriteLine($"RECEIVED_AT: {DateTime.UtcNow}, MESSAGE: {message}");
        }
    }

    // TODO: Move this code to "Claims.Auditing" project?
    private class AuditingQueueListener : IQueueListener<AuditMessage>
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
