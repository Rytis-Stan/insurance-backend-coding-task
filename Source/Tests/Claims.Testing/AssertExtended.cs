using Xunit;

namespace Claims.Testing;

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
}
