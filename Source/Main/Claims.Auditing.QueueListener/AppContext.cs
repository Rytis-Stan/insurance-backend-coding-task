using BuildingBlocks.MessageQueues;
using BuildingBlocks.MessageQueues.RabbitMq;
using Claims.Auditing.MessageQueueBased;
using Claims.Auditing.PersistenceBased;
using Claims.Auditing.QueueListener.Configuration;
using Claims.Auditing.QueueListener.IO;
using Claims.Persistence.Auditing;
using Microsoft.Extensions.Logging;

namespace Claims.Auditing.QueueListener;

public class AppContext : IAppContext
{
    public ILogger Logger { get; }
    public IAuditDatabase Database { get; }
    public IReceivingQueue<AuditMessage> Queue { get; }
    public IQueueListener<AuditMessage> QueueListener { get; }
    public IConsole Console { get; }

    public AppContext(AppConfiguration configuration)
    {
        Logger = CreateLogger();
        Database = CreateDatabase(configuration);
        Queue = CreateReceivingQueue(configuration.RabbitMq);
        QueueListener = RootQueueListener(Database, Logger);
        Console = new SystemConsole();
    }

    private static ILogger CreateLogger()
    {
        return LoggerFactory
            .Create(builder => builder.AddConsole())
            .CreateLogger<LoggingQueueListener<AuditMessage>>();
    }

    private static IAuditDatabase CreateDatabase(AppConfiguration configuration)
    {
        return new EntityFrameworkAuditDatabase(configuration.ConnectionString);
    }

    private static IReceivingQueue<AuditMessage> CreateReceivingQueue(RabbitMqConfiguration configuration)
    {
        return new RabbitMqReceivingQueue<AuditMessage>(configuration.HostName, configuration.QueueName);
    }

    public static IQueueListener<AuditMessage> RootQueueListener(IAuditDatabase database, ILogger logger)
    {
        return new CompositeQueueListener<AuditMessage>(
            new LoggingQueueListener<AuditMessage>(logger),
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
}