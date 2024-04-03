namespace Claims.Auditing.MessageQueues;

public interface IInactiveSendingQueue<in TMessage>
{
    ISendingQueue<TMessage> Activate();
}