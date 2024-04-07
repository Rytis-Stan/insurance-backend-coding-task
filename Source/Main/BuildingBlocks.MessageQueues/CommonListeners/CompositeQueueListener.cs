namespace BuildingBlocks.MessageQueues.CommonListeners;

public class CompositeQueueListener<TMessage> : IQueueListener<TMessage>
{
    private readonly IQueueListener<TMessage>[] _children;

    public CompositeQueueListener(params IQueueListener<TMessage>[] children)
    {
        _children = children;
    }

    public void OnMessageReceived(TMessage message)
    {
        Array.ForEach(_children, child => child.OnMessageReceived(message));
    }
}
