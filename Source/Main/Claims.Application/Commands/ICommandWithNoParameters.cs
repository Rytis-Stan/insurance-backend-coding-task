namespace Claims.Application.Commands;

public interface ICommandWithNoParameters<TResponse>
{
    Task<TResponse> ExecuteAsync();
}
