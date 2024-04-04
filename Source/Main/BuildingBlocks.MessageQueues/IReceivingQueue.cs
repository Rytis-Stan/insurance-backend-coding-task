namespace BuildingBlocks.MessageQueues;

public interface IReceivingQueue<out TMessage> : IDisposable
{
    void OnReceived(Action<TMessage> action);
}