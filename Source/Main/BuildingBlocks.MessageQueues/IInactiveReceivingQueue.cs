namespace BuildingBlocks.MessageQueues;

public interface IInactiveReceivingQueue<out TMessage>
{
    IReceivingQueue<TMessage> Connect();
}