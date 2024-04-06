using RabbitMQ.Client;

namespace BuildingBlocks.MessageQueues.RabbitMq;

public abstract class RabbitMqMessageQueue<TActiveQueue>
{
    private readonly string _hostName;
    private readonly string _queueName;

    protected RabbitMqMessageQueue(string hostName, string queueName)
    {
        _hostName = hostName;
        _queueName = queueName;
    }

    public TActiveQueue Connect()
    {
        var factory = new ConnectionFactory { HostName = _hostName };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        EnsureQueueConstructed(channel);
        return CreateConnectedQueue(connection, channel, _queueName);
    }

    private void EnsureQueueConstructed(IModel channel)
    {
        channel.QueueDeclare(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    protected abstract TActiveQueue CreateConnectedQueue(IConnection connection, IModel channel, string queueName);

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
}
