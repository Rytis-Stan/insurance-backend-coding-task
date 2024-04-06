namespace BuildingBlocks.MessageQueues;

public interface IReceivingQueue<out TMessage>
{
    IConnectedReceivingQueue Connect(IQueueListener<TMessage> listener);
}