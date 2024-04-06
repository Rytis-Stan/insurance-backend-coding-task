namespace BuildingBlocks.MessageQueues;

public interface IInactiveSendingQueue<in TMessage>
{
    ISendingQueue<TMessage> Connect();
}