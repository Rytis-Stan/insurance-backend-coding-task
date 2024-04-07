namespace BuildingBlocks.MessageQueues;

public interface IConnectedSendingQueue<in TMessage> : IDisposable
{
    void Send(TMessage message);
}
