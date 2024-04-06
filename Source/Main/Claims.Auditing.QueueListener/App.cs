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
        using var messageQueue = ConnectToQueue(_configuration.RabbitMq, RootListener(auditDatabase));
        messageQueue.StartListening();
    }

    private static CompositeQueueListener<AuditMessage> RootListener(IAuditDatabase database)
    {
        return new CompositeQueueListener<AuditMessage>(
            new LoggingQueueListener<AuditMessage>(),
            new AuditingQueueListener(AuditorsByAuditEntityKind(database))
        );
    }

    private static Dictionary<AuditEntityKind, IHttpRequestAuditor> AuditorsByAuditEntityKind(IAuditDatabase database)
    {
        return new Dictionary<AuditEntityKind, IHttpRequestAuditor>
        {
            { AuditEntityKind.Cover, new PersistingCoverAuditor(database.CoverAuditRepository) },
            { AuditEntityKind.Claim, new PersistingClaimAuditor(database.ClaimAuditRepository) }
        };
    }

    private static IConnectedReceivingQueue ConnectToQueue(RabbitMqConfiguration configuration, IQueueListener<AuditMessage> listener)
    {
        return new RabbitMqReceivingQueue<AuditMessage>(configuration.HostName, configuration.QueueName).Connect(listener);
    }
}
