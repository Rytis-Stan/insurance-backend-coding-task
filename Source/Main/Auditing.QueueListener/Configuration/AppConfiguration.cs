#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Microsoft.Extensions.Configuration;

namespace Auditing.QueueListener.Configuration;

public class AppConfiguration
{
    public string ConnectionString { get; init; }
    public RabbitMqConfiguration RabbitMq { get; init; }

    public static AppConfiguration FromAppSettings()
    {
        return new AppConfiguration(
            // ReSharper disable once StringLiteralTypo
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
        );
    }

    private AppConfiguration(IConfiguration configuration)
    {
        configuration.Bind(this);
    }
}
