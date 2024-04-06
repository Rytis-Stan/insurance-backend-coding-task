﻿using System.Text.Json;
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

    public IConnectedReceivingQueue<TMessage> Connect()
    {
        var (connection, channel, queueName) = DeclareQueueAndConnectToIt();
        return new ConnectedRabbitMqReceivingQueue(connection, channel, queueName);
    }

    private class ConnectedRabbitMqReceivingQueue : ConnectedRabbitMqMessageQueue, IConnectedReceivingQueue<TMessage>
    {
        public ConnectedRabbitMqReceivingQueue(IConnection connection, IModel channel, string queueName)
            : base(connection, channel, queueName)
        {
        }

        public void OnReceived(Action<TMessage> action)
        {
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (_, e) =>
            {
                var message = ToMessage(e.Body);
                action(message);
                Channel.BasicAck(e.DeliveryTag, multiple: false);
            };

            Channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: consumer
            );
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