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

    public IConnectedReceivingQueue Connect(IQueueListener<TMessage> listener)
    {
        var (connection, channel, queueName) = DeclareQueueAndConnectToIt();
        return new ConnectedRabbitMqReceivingQueue(connection, channel, queueName, listener);
    }

    private class ConnectedRabbitMqReceivingQueue : ConnectedRabbitMqMessageQueue, IConnectedReceivingQueue
    {
        private readonly IQueueListener<TMessage> _listener;

        public ConnectedRabbitMqReceivingQueue(IConnection connection, IModel channel, string queueName, IQueueListener<TMessage> listener)
            : base(connection, channel, queueName)
        {
            _listener = listener;
        }

        public void StartListening()
        {
            Channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: CreateConsumer()
            );
        }

        private IBasicConsumer CreateConsumer()
        {
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (_, e) =>
            {
                var message = ToMessage(e.Body);
                _listener.OnMessageReceived(message);
                Channel.BasicAck(e.DeliveryTag, multiple: false);
            };
            return consumer;
        }

        private static TMessage ToMessage(ReadOnlyMemory<byte> bytes)
        {
            var messageJsonBytes = bytes.ToArray();
            var messageJson = Encoding.UTF8.GetString(messageJsonBytes);
            var message = JsonSerializer.Deserialize<TMessage>(messageJson);
            if (message == null)
            {
                // TODO: Improve error handling (change thrown exception type, etc.).
                throw new Exception("Invalid message received.");
            }
            return message;
        }
    }
}