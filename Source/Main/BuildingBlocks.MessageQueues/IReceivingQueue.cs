namespace BuildingBlocks.MessageQueues;

public interface IReceivingQueue
{
    IConnectedReceivingQueue StartListening();
}