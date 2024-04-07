using System.Text.Json;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BuildingBlocks.MessageQueues.RabbitMq;

public class RabbitMqReceivingQueue<TMessage> : RabbitMqMessageQueue, IReceivingQueue<TMessage>
{
    public RabbitMqReceivingQueue(string hostName, string queueName)
        : base(hostName, queueName)
    {
    }

    public IConnectedReceivingQueue StartListening(IQueueListener<TMessage> listener)
    {
        var (connection, channel, queueName) = DeclareQueueAndConnectToIt();
        channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: CreateConsumer(channel, listener)
        );
        return new ConnectedRabbitMqReceivingQueue(connection, channel);
    }

    private static IBasicConsumer CreateConsumer(IModel channel, IQueueListener<TMessage> listener)
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, e) =>
        {
            var message = ToMessage(e.Body);
            listener.OnMessageReceived(message);
            channel.BasicAck(e.DeliveryTag, multiple: false);
        };
        return consumer;
    }

    private static TMessage ToMessage(ReadOnlyMemory<byte> bytes)
    {
        var messageJsonBytes = bytes.ToArray();
        var messageJson = Encoding.UTF8.GetString(messageJsonBytes);
        var message = JsonSerializer.Deserialize<TMessage>(messageJson);
        
        // TODO: Improve error handling (change thrown exception type, etc.).
        return message == null
            ? throw new Exception("Invalid message received.")
            : message;
    }

    private class ConnectedRabbitMqReceivingQueue : ConnectedRabbitMqMessageQueue, IConnectedReceivingQueue
    {
        public ConnectedRabbitMqReceivingQueue(IConnection connection, IModel channel)
            : base(connection, channel)
        {
        }
    }
}