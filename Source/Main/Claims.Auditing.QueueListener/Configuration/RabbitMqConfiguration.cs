#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Claims.Auditing.QueueListener.Configuration;

public class RabbitMqConfiguration
{
    public string HostName { get; init; }
    public string QueueName { get; init; }
}
