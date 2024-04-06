namespace Claims.Application;

public class ValidationException : Exception
{
    public ValidationException(string? message)
        : base(message)
    {
    }
}
