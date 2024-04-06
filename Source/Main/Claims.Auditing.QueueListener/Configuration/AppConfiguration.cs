using Microsoft.Extensions.Configuration;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Claims.Auditing.QueueListener.Configuration;

public class AppConfiguration
{
    public string ConnectionString { get; init; }
    public RabbitMqConfiguration RabbitMq { get; init; }

    public AppConfiguration(IConfiguration configuration)
    {
        configuration.Bind(this);
    }
}
