namespace BuildingBlocks.MessageQueues;

public interface ISendingQueue<in TMessage>
{
    IConnectedSendingQueue<TMessage> StartSending();
}
