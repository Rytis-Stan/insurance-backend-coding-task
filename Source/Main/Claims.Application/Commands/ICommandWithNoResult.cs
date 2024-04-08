namespace Claims.Application.Commands;

public interface ICommandWithNoResult<in TArgs>
{
    Task ExecuteAsync(TArgs args);
}
