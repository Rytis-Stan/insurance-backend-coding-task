namespace Claims.Application.Commands;

public interface ICommandWithNoResult<in TRequest>
{
    Task ExecuteAsync(TRequest request);
}
