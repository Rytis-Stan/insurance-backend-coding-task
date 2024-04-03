using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace Claims.Auditing;

public abstract class MessageQueueAuditor : IHttpRequestAuditor
{
    private readonly ISendingQueue<AuditMessage> _sendingQueue;
    private readonly AuditEntityKind _entityKind;

    protected MessageQueueAuditor(ISendingQueue<AuditMessage> sendingQueue, AuditEntityKind entityKind)
    {
        _sendingQueue = sendingQueue;
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
        _sendingQueue.Send(new AuditMessage(_entityKind, entityId, httpRequestType));
    }
}

public interface IUninitializedMessageQueue
{
    IMessageQueue Initialize();
}

public interface IMessageQueue : IDisposable
{
    ISendingQueue<AuditMessage> SendingQueue();
    IReceivingQueue<AuditMessage> ReceivingQueue();
}

public class UninitializedRabbitMqMessageQueue : IUninitializedMessageQueue
{
    private readonly string _hostName;
    private readonly string _queueName;

    public UninitializedRabbitMqMessageQueue()
        : this("localhost", "Claims.AuditQueue")
    {
    }

    private UninitializedRabbitMqMessageQueue(string hostName, string queueName)
    {
        _hostName = hostName;
        _queueName = queueName;
    }

    public IMessageQueue Initialize()
    {
        var factory = new ConnectionFactory { HostName = _hostName };
        /*using*/
        var connection = factory.CreateConnection();
        /*using*/
        var channel = connection.CreateModel();

        // TODO: Move the queue name (and some options???) to the configuration file!
        channel.QueueDeclare(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        return new RabbitMqMessageQueue(connection, channel, _queueName);
    }
}

public class RabbitMqMessageQueue : IMessageQueue
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    public RabbitMqMessageQueue(IConnection connection, IModel channel, string queueName)
    {
        _connection = connection;
        _channel = channel;
        _queueName = queueName;
    }

    public ISendingQueue<AuditMessage> SendingQueue()
    {
        return new RabbitMqSendingQueue<AuditMessage>(_channel, _queueName);
    }

    public IReceivingQueue<AuditMessage> ReceivingQueue()
    {
        return new RabbitMqReceivingQueue<AuditMessage>(_channel, _queueName);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}

public interface ISendingQueue<in TMessage>
{
    void Send(TMessage message);
}

public interface IReceivingQueue<out TMessage>
{
    void OnReceived(Action<TMessage> action);
}

public class RabbitMqSendingQueue<TMessage> : ISendingQueue<TMessage>
{
    private readonly IModel _channel;
    private readonly string _queueName;

    public RabbitMqSendingQueue(IModel channel, string queueName)
    {
        _channel = channel;
        _queueName = queueName;
    }

    public void Send(TMessage message)
    {
        var messageJson = JsonSerializer.Serialize(message);
        var messageJsonBytes = Encoding.UTF8.GetBytes(messageJson);
        _channel.BasicPublish(
            exchange: "",
            routingKey: _queueName,
            basicProperties: null,
            body: messageJsonBytes
        );
    }
}

public class RabbitMqReceivingQueue<TMessage> : IReceivingQueue<TMessage>
{
    private readonly IModel _channel;
    private readonly string _queueName;

    public RabbitMqReceivingQueue(IModel channel, string queueName)
    {
        _channel = channel;
        _queueName = queueName;
    }

    public void OnReceived(Action<TMessage> action)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (_, e) =>
        {
            var message = ToMessage(e.Body);
            action(message);
            _channel.BasicAck(e.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(
            queue: _queueName,
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
    public MessageQueueClaimAuditor(ISendingQueue<AuditMessage> sendingQueue)
        : base(sendingQueue, AuditEntityKind.Claim)
    {
    }
}

public class MessageQueueCoverAuditor : MessageQueueAuditor, ICoverAuditor
{
    public MessageQueueCoverAuditor(ISendingQueue<AuditMessage> sendingQueue)
        : base(sendingQueue, AuditEntityKind.Cover)
    {
    }
}

public record AuditMessage(AuditEntityKind EntityType, Guid EntityId, HttpRequestType HttpRequestType);