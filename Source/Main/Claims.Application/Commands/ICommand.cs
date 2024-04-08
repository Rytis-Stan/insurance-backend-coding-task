namespace Claims.Application.Commands;

public interface ICommand<in TArgs, TResult>
{
    Task<TResult> ExecuteAsync(TArgs args);
}
