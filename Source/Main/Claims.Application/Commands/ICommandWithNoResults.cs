namespace Claims.Application.Commands;

public interface ICommandWithNoResults<in TRequest>
{
    Task ExecuteAsync(TRequest request);
}
