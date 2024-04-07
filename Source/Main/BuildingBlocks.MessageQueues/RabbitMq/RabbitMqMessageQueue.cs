using RabbitMQ.Client;

namespace BuildingBlocks.MessageQueues.RabbitMq;

/// <summary>
/// Base class for a RabbitMQ message queue in an unconnected state.
/// </summary>
public abstract class RabbitMqMessageQueue
{
    private readonly string _hostName;
    private readonly string _queueName;

    protected RabbitMqMessageQueue(string hostName, string queueName)
    {
        _hostName = hostName;
        _queueName = queueName;
    }

    /// <summary>
    /// A utility method designed to be called by derived sending and receiving
    /// queue classes. Establishes a connection to the underlying RabbitMQ queue
    /// resource. But before that, if the queue resource does not exist, this method
    /// creates it. Based on the current design, both the sending queue and receiving
    /// queue classes cause the RabbitMQ queue resource to be created, but that might
    /// need to be changed in the future.
    /// </summary>
    /// <returns></returns>
    protected QueueInfo DeclareQueueAndConnectToIt()
    {
        var factory = new ConnectionFactory { HostName = _hostName };
        var connection = factory.CreateConnection()!;
        var channel = connection.CreateModel()!;
        DeclareQueue(channel);
        return new QueueInfo(connection, channel, _queueName);
    }

    private void DeclareQueue(IModel channel)
    {
        channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    protected record QueueInfo(IConnection Connection, IModel Channel, string QueueName);

    /// <summary>
    /// Base class for RabbitMQ message queues in a connected state.
    /// </summary>
    protected abstract class ConnectedRabbitMqMessageQueue : IDisposable
    {
        private readonly IConnection _connection;
        protected readonly IModel Channel;

        protected ConnectedRabbitMqMessageQueue(IConnection connection, IModel channel)
        {
            _connection = connection;
            Channel = channel;
        }

        public void Dispose()
        {
            Channel.Dispose();
            _connection.Dispose();
        }
    }
}
