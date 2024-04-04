namespace Claims.Application.Repositories;

public interface IRepository<in TNewObjectInfo, TObject>
{
    Task<TObject> AddAsync(TNewObjectInfo newObject);

    // NOTE: Using the term "Find" instead of "Get" to imply that the result
    // might not be available, in which case a null value is returned.
    Task<TObject?> FindByIdAsync(Guid id);

    Task<IEnumerable<TObject>> GetAllAsync();
    Task<TObject?> DeleteByIdAsync(Guid id);
}
