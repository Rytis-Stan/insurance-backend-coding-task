using Xunit;

namespace Claims.Tests;

public static class AssertExtended
{
    public static async Task ThrowsArgumentExceptionAsync(Func<Task> action, string expectedMessage)
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(action);
        Assert.Equal(expectedMessage, exception.Message);
    }
}
