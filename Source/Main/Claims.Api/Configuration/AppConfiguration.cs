namespace Claims.Api.Configuration;

public class AppConfiguration
{
    public CosmosDbConfiguration CosmosDb { get; init; }
    public RabbitMqConfiguration RabbitMq { get; init; }

    public static AppConfiguration FromConfiguration(IConfiguration configuration)
    {
        var appConfiguration = new AppConfiguration();
        configuration.Bind(appConfiguration);
        return appConfiguration;
    }
}
