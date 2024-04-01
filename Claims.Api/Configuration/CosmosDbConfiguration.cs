namespace Claims.Api.Configuration;

public class CosmosDbConfiguration
{
    private readonly IConfigurationSection _configurationSection;

    public string DatabaseName => _configurationSection.GetSection("DatabaseName").Value;
    public string Account => _configurationSection.GetSection("Account").Value;
    public string Key => _configurationSection.GetSection("Key").Value;

    public CosmosDbConfiguration(IConfigurationSection configurationSection)
    {
        _configurationSection = configurationSection;
    }
}
