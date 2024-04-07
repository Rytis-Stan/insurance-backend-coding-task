using BuildingBlocks.MessageQueues;
using BuildingBlocks.MessageQueues.RabbitMq;
using Claims.Auditing.MessageQueueBased;
using Claims.Auditing.PersistenceBased;
using Claims.Auditing.QueueListener.Configuration;
using Claims.Persistence.Auditing;
using Microsoft.Extensions.Logging;

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
        var logger = Logger();

        logger.LogInformation("Starting to migrate the auditing database.");
        
        MigrateAuditDatabase(_configuration.ConnectionString);

        logger.LogInformation("Migration finished. Starting to listed to messages.");

        StartListeningToAuditMessages(logger);
        
        logger.LogInformation("Press [Enter] to quit.");
        Console.ReadLine();
    }

    // TODO: Ensure that the same audit database instance gets reused both for migration and for setting up the auditors.
    private static void MigrateAuditDatabase(string connectionString)
    {
        new EntityFrameworkAuditDatabase(connectionString).Migrate();
    }

    // TODO: Return the disposables?
    private void StartListeningToAuditMessages(ILogger logger)
    {
        using var auditDatabase = new EntityFrameworkAuditDatabase(_configuration.ConnectionString);
        using var messageQueue = ConnectToQueue(_configuration.RabbitMq, RootListener(auditDatabase, logger));
        messageQueue.StartListening();
    }

    private static CompositeQueueListener<AuditMessage> RootListener(IAuditDatabase database, ILogger logger)
    {
        return new CompositeQueueListener<AuditMessage>(
            new LoggingQueueListener<AuditMessage>(logger),
            new AuditingQueueListener(AuditorsByAuditEntityKind(database))
        );
    }

    private static ILogger Logger()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        return loggerFactory.CreateLogger<LoggingQueueListener<AuditMessage>>();
    }

    // private static ILogger<LoggingQueueListener<AuditMessage>> Logger()
    // {
    //     var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    //     return loggerFactory.CreateLogger<LoggingQueueListener<AuditMessage>>();
    // }

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
