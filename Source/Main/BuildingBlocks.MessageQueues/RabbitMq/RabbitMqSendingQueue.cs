using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace BuildingBlocks.MessageQueues.RabbitMq;

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