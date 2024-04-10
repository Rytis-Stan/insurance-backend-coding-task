using Auditing.Persistence;
using Auditing.QueueAgent.IO;
using BuildingBlocks.MessageQueues;
using Microsoft.Extensions.Logging;

namespace Auditing.QueueAgent;

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
            context.Queues,
            context.Console
        );
    }

    private static void Run(ILogger logger, IAuditDatabase database, IEnumerable<IReceivingQueue> queues, IConsole console)
    {
        logger.LogInformation("Starting to migrate the auditing database.");
        database.Migrate();

        logger.LogInformation("Migration finished.");
        logger.LogInformation("Starting to listed to messages.");

        var connectedQueues = queues.Select(x => x.StartListening()).ToList();

        logger.LogInformation("Press [Enter] to quit.");
        console.WaitTillEnterKeyPressed();

        connectedQueues.ForEach(x => x.Dispose());
    }
}
