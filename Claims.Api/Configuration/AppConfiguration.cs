namespace Claims.Api.Configuration;

public class AppConfiguration
{
    public string ConnectionString { get; set; }
    public CosmosDbConfiguration CosmosDb { get; set; }
}
