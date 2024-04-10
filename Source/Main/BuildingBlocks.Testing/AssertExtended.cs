using Xunit;

namespace BuildingBlocks.Testing;

public static class AssertExtended
{
    public static void Throws<TException>(Action action, string expectedMessage)
        where TException : Exception
    {
        var exception = Assert.Throws<TException>(action);
        Assert.Equal(expectedMessage, exception.Message);
    }

    public static async Task ThrowsAsync<TException>(Func<Task> action, string expectedMessage)
        where TException : Exception
    {
        var exception = await Assert.ThrowsAsync<TException>(action);
        Assert.Equal(expectedMessage, exception.Message);
    }

    public static void EqualIgnoreOrder<T, TSortKey>(IEnumerable<T>? expected, IEnumerable<T>? actual, Func<T, TSortKey> keyToSortBy)
    {
        Assert.NotNull(expected);
        Assert.NotNull(actual);
        var expectedOrdered = expected.OrderBy(keyToSortBy).ToList();
        var actualOrdered = actual.OrderBy(keyToSortBy).ToList();
        Assert.Equal(expectedOrdered, actualOrdered);
    }
}
