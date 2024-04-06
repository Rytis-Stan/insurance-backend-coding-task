using RabbitMQ.Client;

namespace BuildingBlocks.MessageQueues.RabbitMq;

public class RabbitMqSendingQueue<TMessage>
    : RabbitMqMessageQueue<ISendingQueue<TMessage>>, IInactiveSendingQueue<TMessage>
{
    public RabbitMqSendingQueue(string hostName, string queueName)
        : base(hostName, queueName)
    {
    }

    protected override ISendingQueue<TMessage> CreateActiveQueue(
        IConnection connection, IModel channel, string queueName)
    {
        return new ConnectedRabbitMqSendingQueue<TMessage>(connection, channel, queueName);
    }
}