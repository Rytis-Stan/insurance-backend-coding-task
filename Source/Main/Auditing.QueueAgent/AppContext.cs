﻿using Auditing.Auditors.MessageQueueBased;
using Auditing.Auditors.PersistenceBased;
using Auditing.Persistence;
using Auditing.QueueAgent.Configuration;
using Auditing.QueueAgent.IO;
using BuildingBlocks.MessageQueues;
using BuildingBlocks.MessageQueues.CommonListeners;
using BuildingBlocks.MessageQueues.RabbitMq;
using BuildingBlocks.Temporal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Auditing.QueueAgent;

public class AppContext : IAppContext
{
    public ILogger Logger { get; }
    public IAuditDatabase Database { get; }
    public IEnumerable<IReceivingQueue> Queues { get; }
    public IConsole Console { get; }

    public AppContext(AppConfiguration configuration)
    {
        Logger = CreateLogger();
        Database = CreateDatabase(configuration);
        Queues = CreateQueues(configuration.RabbitMq, Logger, Database);
        Console = new SystemConsole();
    }

    public void Dispose()
    {
        Database.Dispose();
        GC.SuppressFinalize(this);
    }

    private static ILogger CreateLogger()
    {
        return LoggerFactory
            .Create(builder => builder.AddConsole())
            .CreateLogger<LoggingQueueListener<AuditMessage>>();
    }

    private static IAuditDatabase CreateDatabase(AppConfiguration configuration)
    {
        return new EntityFrameworkAuditDatabase(CreateAuditContext(configuration.ConnectionString), new Clock());
    }

    public static AuditContext CreateAuditContext(string connectionString)
    {
        return new AuditContext(DbContextOptions(connectionString));
    }

    private static DbContextOptions<AuditContext> DbContextOptions(string connectionString)
    {
        return new DbContextOptionsBuilder<AuditContext>().UseSqlServer(connectionString).Options;
    }

    private static IEnumerable<IReceivingQueue> CreateQueues(RabbitMqConfiguration configuration, ILogger logger, IAuditDatabase database)
    {
        return new[]
        {
            CreateQueue(configuration.HostName, configuration.QueueNames.CoverAudit, logger, database.CoverAuditRepository),
            CreateQueue(configuration.HostName, configuration.QueueNames.ClaimAudit, logger, database.ClaimAuditRepository),
        };
    }

    private static IReceivingQueue CreateQueue(string hostName, string queueName, ILogger logger, IAuditRepository repository)
    {
        return new RabbitMqReceivingQueue<AuditMessage>(hostName, queueName,
            new CompositeQueueListener<AuditMessage>(
                new LoggingQueueListener<AuditMessage>(logger, queueName),
                new AuditingQueueListener(new PersistingAuditor(repository))
            )
        );
    }
}