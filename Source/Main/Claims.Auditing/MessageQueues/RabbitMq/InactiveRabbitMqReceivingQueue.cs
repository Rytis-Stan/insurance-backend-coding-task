using RabbitMQ.Client;

namespace Claims.Auditing.MessageQueues.RabbitMq;

public class InactiveRabbitMqReceivingQueue<TMessage>
    : InactiveRabbitMqMessageQueue<IReceivingQueue<TMessage>>, IInactiveReceivingQueue<TMessage>
{
    public InactiveRabbitMqReceivingQueue(string hostName, string queueName)
        : base(hostName, queueName)
    {
    }

    protected override IReceivingQueue<TMessage> CreateActiveQueue(
        IConnection connection, IModel channel, string queueName)
    {
        return new RabbitMqReceivingQueue<TMessage>(connection, channel, queueName);
    }
}