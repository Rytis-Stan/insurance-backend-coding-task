namespace Claims.Application.Commands;

public interface ICommandWithNoArgs<TResult>
{
    Task<TResult> ExecuteAsync();
}
