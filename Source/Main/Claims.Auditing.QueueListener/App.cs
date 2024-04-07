using BuildingBlocks.MessageQueues;
using Claims.Auditing.MessageQueueBased;
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
        Run(
            _context.Logger,
            _context.Database,
            _context.Queue,
            _context.QueueListener,
            _context.Console
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
