namespace Claims.Api.Configuration;

public class AppConfiguration
{
    public string ConnectionString { get; set; }
    public CosmosDbConfiguration CosmosDb { get; set; }

    public static AppConfiguration FromConfiguration(IConfiguration configuration)
    {
        var appConfiguration = new AppConfiguration();
        configuration.Bind(appConfiguration);
        return appConfiguration;
    }
}
