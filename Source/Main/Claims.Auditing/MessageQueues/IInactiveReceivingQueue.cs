namespace Claims.Auditing.MessageQueues;

public interface IInactiveReceivingQueue<out TMessage>
{
    IReceivingQueue<TMessage> Activate();
}