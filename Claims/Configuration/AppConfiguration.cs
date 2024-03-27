namespace Claims.Configuration;

public class AppConfiguration
{
    private readonly ConfigurationManager _configuration;

    public AppConfiguration(ConfigurationManager configuration)
    {
        _configuration = configuration;
    }

    public string ConnectionString => _configuration.GetConnectionString("DefaultConnection");
    public CosmosDbConfiguration CosmosDb => new CosmosDbConfiguration(_configuration.GetSection("CosmosDb"));
}
