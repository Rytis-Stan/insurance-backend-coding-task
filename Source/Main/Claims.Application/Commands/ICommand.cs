namespace Claims.Application.Commands;

public interface ICommand<in TRequest, TResponse>
{
    Task<TResponse> ExecuteAsync(TRequest request);
}
