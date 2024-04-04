using Microsoft.Extensions.Configuration;

namespace Claims.AuditDaemon.Configuration;

public class AppConfiguration
{
    public string ConnectionString { get; init; }
    public RabbitMqConfiguration RabbitMq { get; init; }

    public static AppConfiguration FromConfiguration(IConfiguration configuration)
    {
        var appConfiguration = new AppConfiguration();
        configuration.Bind(appConfiguration);
        return appConfiguration;
    }
}
