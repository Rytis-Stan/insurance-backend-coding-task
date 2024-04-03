using RabbitMQ.Client;

namespace Claims.Auditing.MessageQueues.RabbitMq;

public class InactiveRabbitMqSendingQueue<TMessage>
    : InactiveRabbitMqMessageQueue<ISendingQueue<TMessage>>, IInactiveSendingQueue<TMessage>
{
    public InactiveRabbitMqSendingQueue()
        : base("localhost", "Claims.AuditQueue")
    {
    }

    protected override ISendingQueue<TMessage> CreateActiveQueue(
        IConnection connection, IModel channel, string queueName)
    {
        return new RabbitMqSendingQueue<TMessage>(connection, channel, queueName);
    }
}