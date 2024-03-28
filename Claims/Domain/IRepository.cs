namespace Claims.Domain;

public interface IRepository<in TNewObjectInfo, TObject>
{
    Task<TObject> AddAsync(TNewObjectInfo item);
    Task<TObject?> GetByIdAsync(Guid id);
    Task<IEnumerable<TObject>> GetAllAsync();
    Task<TObject> DeleteAsync(Guid id);
}
