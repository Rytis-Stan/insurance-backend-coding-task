using Microsoft.Extensions.Logging;

namespace BuildingBlocks.MessageQueues.CommonListeners;

public class LoggingQueueListener<TMessage> : IQueueListener<TMessage>
{
    private readonly ILogger _logger;

    public LoggingQueueListener(ILogger logger)
    {
        _logger = logger;
    }

    public void OnMessageReceived(TMessage message)
    {
        _logger.LogInformation("Received message: {Message}", message);
    }
}
