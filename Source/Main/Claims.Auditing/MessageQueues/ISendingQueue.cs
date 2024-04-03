namespace Claims.Auditing.MessageQueues;

public interface ISendingQueue<in TMessage> : IDisposable
{
    void Send(TMessage message);
}