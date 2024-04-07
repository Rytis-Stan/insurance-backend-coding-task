using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace BuildingBlocks.MessageQueues.RabbitMq;

/// <summary>
/// Represents a RabbitMQ message queue in an unconnected state, which needs to establish
/// a connection to the underlying RabbitMQ queue by calling the "StartListening" method.
/// Once that method is called, a new queue kind of queue object gets returned that can
/// be used to send messages to the underlying RabbitMQ queue.
/// </summary>
public class RabbitMqSendingQueue<TMessage> : RabbitMqMessageQueue, ISendingQueue<TMessage>
{
    public RabbitMqSendingQueue(string hostName, string queueName)
        : base(hostName, queueName)
    {
    }

    public IConnectedSendingQueue<TMessage> StartSending()
    {
        var (connection, channel, queueName) = DeclareQueueAndConnectToIt();
        return new ConnectedRabbitMqSendingQueue(connection, channel, queueName);
    }

    private class ConnectedRabbitMqSendingQueue : ConnectedRabbitMqMessageQueue, IConnectedSendingQueue<TMessage>
    {
        private readonly string _queueName;

        public ConnectedRabbitMqSendingQueue(IConnection connection, IModel channel, string queueName)
            : base(connection, channel)
        {
            _queueName = queueName;
        }

        public void Send(TMessage message)
        {
            var messageJson = JsonSerializer.Serialize(message);
            var messageJsonBytes = Encoding.UTF8.GetBytes(messageJson);
            Channel.BasicPublish(
                exchange: "",
                routingKey: _queueName,
                basicProperties: null,
                body: messageJsonBytes
            );
        }
    }
}