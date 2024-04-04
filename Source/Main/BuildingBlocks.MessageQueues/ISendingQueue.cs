namespace BuildingBlocks.MessageQueues;

public interface ISendingQueue<in TMessage> : IDisposable
{
    void Send(TMessage message);
}