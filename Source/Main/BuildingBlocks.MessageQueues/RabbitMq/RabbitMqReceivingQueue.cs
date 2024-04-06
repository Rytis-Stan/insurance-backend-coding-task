using RabbitMQ.Client;

namespace BuildingBlocks.MessageQueues.RabbitMq;

public class RabbitMqReceivingQueue<TMessage>
    : RabbitMqMessageQueue<IReceivingQueue<TMessage>>, IInactiveReceivingQueue<TMessage>
{
    public RabbitMqReceivingQueue(string hostName, string queueName)
        : base(hostName, queueName)
    {
    }

    protected override IReceivingQueue<TMessage> CreateActiveQueue(
        IConnection connection, IModel channel, string queueName)
    {
        return new ConnectedRabbitMqReceivingQueue<TMessage>(connection, channel, queueName);
    }
}