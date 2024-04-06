using RabbitMQ.Client;

namespace BuildingBlocks.MessageQueues.RabbitMq;

public abstract class ConnectedRabbitMqMessageQueue
{
    private readonly IConnection _connection;
    protected readonly IModel Channel;
    protected readonly string QueueName;

    protected ConnectedRabbitMqMessageQueue(IConnection connection, IModel channel, string queueName)
    {
        _connection = connection;
        Channel = channel;
        QueueName = queueName;
    }

    public void Dispose()
    {
        Channel.Dispose();
        _connection.Dispose();
    }
}