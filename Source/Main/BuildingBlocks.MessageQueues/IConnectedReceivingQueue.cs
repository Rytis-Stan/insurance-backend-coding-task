namespace BuildingBlocks.MessageQueues;

public interface IConnectedReceivingQueue<out TMessage> : IDisposable
{
    void OnReceived(Action<TMessage> action);
}