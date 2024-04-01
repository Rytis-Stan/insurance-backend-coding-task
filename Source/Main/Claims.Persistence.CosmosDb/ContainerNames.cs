namespace Claims.Persistence.CosmosDb;

internal static class ContainerNames
{
    public static readonly string Claim = nameof(Claim);
    public static readonly string Cover = nameof(Cover);

    public static readonly IEnumerable<string> All = new[] { Claim, Cover };
}
