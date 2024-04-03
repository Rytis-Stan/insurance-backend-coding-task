using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace Claims.Auditing;

public abstract class MessageQueueAuditor : IHttpRequestAuditor
{
    private readonly ISendingQueue<AuditMessage> _queue;
    private readonly AuditEntityKind _entityKind;

    protected MessageQueueAuditor(ISendingQueue<AuditMessage> queue, AuditEntityKind entityKind)
    {
        _queue = queue;
        _entityKind = entityKind;
    }

    public void AuditPost(Guid entityId)
    {
        Audit(entityId, HttpRequestType.Post);
    }

    public void AuditDelete(Guid entityId)
    {
        Audit(entityId, HttpRequestType.Delete);
    }

    private void Audit(Guid entityId, HttpRequestType httpRequestType)
    {
        _queue.Send(new AuditMessage(_entityKind, entityId, httpRequestType));
    }
}

public interface IUninitializedSendingQueue<in TMessage>
{
    ISendingQueue<TMessage> Initialize();
}

public interface IUninitializedReceivingQueue<out TMessage>
{
    IReceivingQueue<TMessage> Initialize();
}

public class UninitializedRabbitMqSendingQueue<TMessage> : RabbitMqMessageQueue<TMessage>, IUninitializedSendingQueue<TMessage>
{
    public UninitializedRabbitMqSendingQueue()
        : base("localhost", "Claims.AuditQueue")
    {
    }

    public ISendingQueue<TMessage> Initialize()
    {
        var (connection, channel, queueName) = DoInitialize();
        return new RabbitMqSendingQueue<TMessage>(connection, channel, queueName);
    }
}

public class RabbitMqMessageQueue<TMessage>
{
    private readonly string _hostName;
    private readonly string _queueName;

    protected RabbitMqMessageQueue(string hostName, string queueName)
    {
        _hostName = hostName;
        _queueName = queueName;
    }

    protected (IConnection, IModel, string) DoInitialize()
    {
        var factory = new ConnectionFactory { HostName = _hostName };
        /*using*/ var connection = factory.CreateConnection();
        /*using*/ IModel channel = connection.CreateModel();

        // TODO: Move the queue name (and some options???) to the configuration file!
        channel.QueueDeclare(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        return (connection, channel, _queueName);
    }
}

public class UninitializedRabbitMqReceivingQueue<TMessage> : RabbitMqMessageQueue<TMessage>, IUninitializedReceivingQueue<TMessage>
{
    public UninitializedRabbitMqReceivingQueue()
        : base("localhost", "Claims.AuditQueue")
    {
    }

    public IReceivingQueue<TMessage> Initialize()
    {
        var (connection, channel, queueName) = DoInitialize();
        return new RabbitMqReceivingQueue<TMessage>(connection, channel, queueName);
    }
}

public abstract class RabbitMqMessageQueue
{
    private readonly IConnection _connection;
    protected readonly IModel Channel;
    protected readonly string QueueName;

    protected RabbitMqMessageQueue(IConnection connection, IModel channel, string queueName)
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

public interface ISendingQueue<in TMessage> : IDisposable
{
    void Send(TMessage message);
}

public interface IReceivingQueue<out TMessage> : IDisposable
{
    void OnReceived(Action<TMessage> action);
}

public class RabbitMqSendingQueue<TMessage> : RabbitMqMessageQueue, ISendingQueue<TMessage>
{
    public RabbitMqSendingQueue(IConnection connection, IModel channel, string queueName)
        : base(connection, channel, queueName)
    {
    }

    public void Send(TMessage message)
    {
        var messageJson = JsonSerializer.Serialize(message);
        var messageJsonBytes = Encoding.UTF8.GetBytes(messageJson);
        Channel.BasicPublish(
            exchange: "",
            routingKey: QueueName,
            basicProperties: null,
            body: messageJsonBytes
        );
    }
}

public class RabbitMqReceivingQueue<TMessage> : RabbitMqMessageQueue, IReceivingQueue<TMessage>
{
    public RabbitMqReceivingQueue(IConnection connection, IModel channel, string queueName)
        : base(connection, channel, queueName)
    {
    }

    public void OnReceived(Action<TMessage> action)
    {
        var consumer = new EventingBasicConsumer(Channel);
        consumer.Received += (_, e) =>
        {
            var message = ToMessage(e.Body);
            action(message);
            Channel.BasicAck(e.DeliveryTag, multiple: false);
        };

        Channel.BasicConsume(
            queue: QueueName,
            autoAck: false,
            consumer: consumer
        );
    }

    private static TMessage ToMessage(ReadOnlyMemory<byte> bytes)
    {
        var messageJsonBytes = bytes.ToArray();
        var messageJson = Encoding.UTF8.GetString(messageJsonBytes);
        var message = JsonSerializer.Deserialize<TMessage>(messageJson);
        if (message == null)
        {
            // TODO: Improve error handling (change thrown exception type, etc.).
            throw new Exception("Invalid message received.");
        }
        return message;
    }
}

public class MessageQueueClaimAuditor : MessageQueueAuditor, IClaimAuditor
{
    public MessageQueueClaimAuditor(ISendingQueue<AuditMessage> queue)
        : base(queue, AuditEntityKind.Claim)
    {
    }
}

public class MessageQueueCoverAuditor : MessageQueueAuditor, ICoverAuditor
{
    public MessageQueueCoverAuditor(ISendingQueue<AuditMessage> queue)
        : base(queue, AuditEntityKind.Cover)
    {
    }
}

public record AuditMessage(AuditEntityKind EntityType, Guid EntityId, HttpRequestType HttpRequestType);