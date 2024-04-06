using Xunit;

namespace Claims.Application.Tests;

public static class AssertExtended
{
    public static async Task ThrowsValidationExceptionAsync(Func<Task> action, string expectedMessage)
    {
        var exception = await Assert.ThrowsAsync<ValidationException>(action);
        Assert.Equal(expectedMessage, exception.Message);
    }
}
