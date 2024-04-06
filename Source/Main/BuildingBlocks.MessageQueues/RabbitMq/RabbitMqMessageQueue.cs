using RabbitMQ.Client;

namespace BuildingBlocks.MessageQueues.RabbitMq;

public abstract class RabbitMqMessageQueue<TConnectedQueue>
{
    private readonly string _hostName;
    private readonly string _queueName;

    protected RabbitMqMessageQueue(string hostName, string queueName)
    {
        _hostName = hostName;
        _queueName = queueName;
    }

    public TConnectedQueue Connect()
    {
        var (connection, channel) = DeclareQueueAndConnectToIt();
        return CreateConnectedQueue(connection, channel, _queueName);
    }

    private (IConnection Connection, IModel Channel) DeclareQueueAndConnectToIt()
    {
        var factory = new ConnectionFactory { HostName = _hostName };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        DeclareQueue(channel);
        return (connection, channel);
    }

    private void DeclareQueue(IModel channel)
    {
        channel.QueueDeclare(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    protected abstract TConnectedQueue CreateConnectedQueue(IConnection connection, IModel channel, string queueName);

    public abstract class ConnectedRabbitMqMessageQueue : IDisposable
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
