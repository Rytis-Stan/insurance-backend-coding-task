namespace BuildingBlocks.MessageQueues;

public class MessageQueueException : Exception
{
    public MessageQueueException(string? message)
        : base(message)
    {
    }
}
