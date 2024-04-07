using BuildingBlocks.MessageQueues;
using Claims.Auditing.MessageQueueBased;
using Claims.Auditing.QueueListener.IO;
using Claims.Persistence.Auditing;
using Microsoft.Extensions.Logging;

namespace Claims.Auditing.QueueListener;

/// <summary>
/// Represents the logic of whole application. Please see the comments of the
/// <see cref="IAppContext" /> interface for more details.
/// </summary>
public class App
{
    private readonly IAppContext _context;

    public App(IAppContext context)
    {
        _context = context;
    }

    public void Run()
    {
        using var context = _context;
        Run(
            context.Logger,
            context.Database,
            context.Queue,
            context.QueueListener,
            context.Console
        );
    }

    private static void Run(ILogger logger, IAuditDatabase database, IReceivingQueue<AuditMessage> queue, IQueueListener<AuditMessage> queueListener, IConsole console)
    {
        logger.LogInformation("Starting to migrate the auditing database.");
        database.Migrate();

        logger.LogInformation("Migration finished.");
        logger.LogInformation("Starting to listed to messages.");

        using var messageQueue = queue.Connect(queueListener);
        messageQueue.StartListening();

        logger.LogInformation("Press [Enter] to quit.");
        console.WaitTillEnterKeyPressed();
    }

    // TODO: Return the disposables?
}
