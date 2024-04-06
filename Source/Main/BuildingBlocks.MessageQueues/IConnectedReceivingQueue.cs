namespace BuildingBlocks.MessageQueues;

public interface IConnectedReceivingQueue : IDisposable
{
    void StartListening();
}