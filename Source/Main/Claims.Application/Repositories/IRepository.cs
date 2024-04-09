namespace Claims.Application.Repositories;

public interface IRepository<in TNewDomainEntityInfo, TDomainEntity>
{
    Task<TDomainEntity> CreateAsync(TNewDomainEntityInfo entityInfo);

    // NOTE: Using the term "Find" instead of "Get" to imply that the result
    // might not be available, in which case a null value is returned.
    Task<TDomainEntity?> FindByIdAsync(Guid id);

    Task<IEnumerable<TDomainEntity>> GetAllAsync();
    Task DeleteByIdAsync(Guid id);
}
