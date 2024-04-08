using Xunit;

namespace Claims.Api.Tests;

// TODO: Find a way to merge this class with the other "AssertExtended" class.
public static class AssertExtended
{
    public static void EqualIgnoreOrder<T, TSortKey>(
        IEnumerable<T> expected, IEnumerable<T> actual, Func<T, TSortKey> keyToSortBy)
    {
        var expectedOrdered = expected.OrderBy(keyToSortBy).ToList();
        var actualOrdered = actual.OrderBy(keyToSortBy).ToList();
        Assert.Equal(expectedOrdered, actualOrdered);
    }
}
