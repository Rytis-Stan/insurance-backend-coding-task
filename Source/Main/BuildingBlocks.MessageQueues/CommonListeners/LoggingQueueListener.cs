using Microsoft.Extensions.Logging;

namespace BuildingBlocks.MessageQueues.CommonListeners;

public class LoggingQueueListener<TMessage> : IQueueListener<TMessage>
{
    private readonly ILogger _logger;
    private readonly string _queueName;

    public LoggingQueueListener(ILogger logger, string queueName)
    {
        _logger = logger;
        _queueName = queueName;
    }

    public void OnMessageReceived(TMessage message)
    {
        _logger.LogInformation("Received message from queue \"{QueueName}\". Message: {Message}", _queueName, message);
    }
}
