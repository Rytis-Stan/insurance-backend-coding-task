#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Claims.Api.Configuration;

public class CosmosDbConfiguration
{
    public string DatabaseName { get; init; }
    public string Account { get; init; }
    public string Key { get; init; }
}
