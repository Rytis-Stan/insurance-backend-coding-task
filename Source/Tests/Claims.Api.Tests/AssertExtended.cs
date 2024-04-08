using Claims.Api.Dto;
using Xunit;

namespace Claims.Api.Tests;

// TODO: Find a way to merge this class with the other "AssertExtended" class.
public static class AssertExtended
{
    public static void EqualIgnoreOrder(IEnumerable<CoverDto>? expected, IEnumerable<CoverDto>? actual)
    {
        EqualIgnoreOrder(expected, actual, x => x.Id);
    }

    public static void EqualIgnoreOrder(IEnumerable<ClaimDto>? expected, IEnumerable<ClaimDto>? actual)
    {
        EqualIgnoreOrder(expected, actual, x => x.Id);
    }

    private static void EqualIgnoreOrder<T, TSortKey>(IEnumerable<T>? expected, IEnumerable<T>? actual, Func<T, TSortKey> keyToSortBy)
    {
        Assert.NotNull(expected);
        Assert.NotNull(actual);
        var expectedOrdered = expected.OrderBy(keyToSortBy).ToList();
        var actualOrdered = actual.OrderBy(keyToSortBy).ToList();
        Assert.Equal(expectedOrdered, actualOrdered);
    }
}
