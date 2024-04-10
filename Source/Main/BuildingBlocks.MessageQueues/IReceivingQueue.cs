namespace BuildingBlocks.MessageQueues;

public interface IReceivingQueue<out TMessage>
{
    IConnectedReceivingQueue StartListening();
}