namespace Claims.Application.Commands;

public interface ICommandWithNoArgs<TResponse>
{
    Task<TResponse> ExecuteAsync();
}
