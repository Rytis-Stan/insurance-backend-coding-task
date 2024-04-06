namespace BuildingBlocks.MessageQueues;

public interface IQueueListener<in TMessage>
{
    void OnMessageReceived(TMessage message);
}
