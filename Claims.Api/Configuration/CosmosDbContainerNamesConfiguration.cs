using System.Collections;

namespace Claims.Api.Configuration;

public class CosmosDbContainerNamesConfiguration : IEnumerable<string>
{
    private readonly IConfigurationSection _configurationSection;

    public string Claim => _configurationSection.GetSection("Claim").Value;
    public string Cover => _configurationSection.GetSection("Cover").Value;

    public CosmosDbContainerNamesConfiguration(IConfigurationSection configurationSection)
    {
        _configurationSection = configurationSection;
    }

    public IEnumerator<string> GetEnumerator()
    {
        return _configurationSection
            .GetChildren()
            .Select(x => x.Value)
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
