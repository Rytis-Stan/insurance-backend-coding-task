using RabbitMQ.Client;

namespace Claims.Auditing.MessageQueues;

public class InactiveRabbitMqReceivingQueue<TMessage>
    : InactiveRabbitMqMessageQueue<IReceivingQueue<TMessage>>, IInactiveReceivingQueue<TMessage>
{
    public InactiveRabbitMqReceivingQueue()
        : base("localhost", "Claims.AuditQueue")
    {
    }

    protected override IReceivingQueue<TMessage> CreateActiveQueue(
        IConnection connection, IModel channel, string queueName)
    {
        return new RabbitMqReceivingQueue<TMessage>(connection, channel, queueName);
    }
}