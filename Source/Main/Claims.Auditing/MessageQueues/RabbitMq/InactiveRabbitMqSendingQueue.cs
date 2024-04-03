using RabbitMQ.Client;

namespace Claims.Auditing.MessageQueues.RabbitMq;

public class InactiveRabbitMqSendingQueue<TMessage>
    : InactiveRabbitMqMessageQueue<ISendingQueue<TMessage>>, IInactiveSendingQueue<TMessage>
{
    public InactiveRabbitMqSendingQueue(string hostName, string queueName)
        : base(hostName, queueName)
    {
    }

    protected override ISendingQueue<TMessage> CreateActiveQueue(
        IConnection connection, IModel channel, string queueName)
    {
        return new RabbitMqSendingQueue<TMessage>(connection, channel, queueName);
    }
}