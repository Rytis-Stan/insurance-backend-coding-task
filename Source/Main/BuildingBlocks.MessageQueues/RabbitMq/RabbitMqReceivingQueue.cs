using System.Text.Json;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BuildingBlocks.MessageQueues.RabbitMq;

/// <summary>
/// Represents a RabbitMQ message queue in an unconnected state, which needs to establish
/// a connection to the underlying RabbitMQ queue resource by calling the "StartListening"
/// method. Once that method is called, a new type of queue object gets returned. The queue
/// will keep receiving new messages until the new queue object gets disposed of.
/// </summary>
public class RabbitMqReceivingQueue<TMessage> : RabbitMqMessageQueue, IReceivingQueue<TMessage>
{
    private readonly IQueueListener<TMessage> _listener;

    public RabbitMqReceivingQueue(string hostName, string queueName, IQueueListener<TMessage> listener)
        : base(hostName, queueName)
    {
        _listener = listener;
    }

    public IConnectedReceivingQueue StartListening()
    {
        var (connection, channel, queueName) = DeclareQueueAndConnectToIt();
        channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: CreateConsumer(channel, _listener, queueName)
        );
        return new ConnectedRabbitMqReceivingQueue(connection, channel);
    }

    private static IBasicConsumer CreateConsumer(IModel channel, IQueueListener<TMessage> listener, string queueName)
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, e) =>
        {
            var message = ToMessage(e.Body, queueName);
            listener.OnMessageReceived(message);
            channel.BasicAck(e.DeliveryTag, multiple: false);
        };
        return consumer;
    }

    private static TMessage ToMessage(ReadOnlyMemory<byte> messageBody, string queueName)
    {
        var messageJsonBytes = messageBody.ToArray();
        var messageJson = Encoding.UTF8.GetString(messageJsonBytes);
        var message = JsonSerializer.Deserialize<TMessage>(messageJson);
        return message == null
            ? throw new MessageQueueException($"Message queue \"{queueName}\" received an invalid message.")
            : message;
    }

    /// <summary>
    /// Represents a RabbitMQ message queue in a connected state that is already reacting to
    /// incoming messages. Since the binding of the queue to an instance of <see cref="IQueueListener{TMessage}"/>
    /// occurs when starting the listening, before the current queue object is constructed,
    /// there is nothing "left to do" for this queue object but only to act as an implementation
    /// of the <see cref="IDisposable"/> interface for the underlying RabbitMQ connection and channel.
    /// </summary>
    private class ConnectedRabbitMqReceivingQueue : ConnectedRabbitMqMessageQueue, IConnectedReceivingQueue
    {
        public ConnectedRabbitMqReceivingQueue(IConnection connection, IModel channel)
            : base(connection, channel)
        {
        }
    }
}