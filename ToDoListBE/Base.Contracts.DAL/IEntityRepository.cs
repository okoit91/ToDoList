using Base.Contracts.Domain;

namespace Base.Contracts.DAL;

public interface IEntityRepository<TEntity>: IEntityRepository<TEntity, Guid>
    where TEntity : class, IDomainEntityId
{
    
}

public interface IEntityRepository<TEntity, TKey>
    where TEntity : class, IDomainEntityId<TKey>
    where TKey : IEquatable<TKey>
{
    
    TEntity Add(TEntity entity);
    TEntity Update(TEntity entity);
    
    Task<TEntity?> UpdateAsync(TEntity entity);
    
    int Remove(TEntity entity);
    int Remove(TKey id);

    
    TEntity? FirstOrDefault(TKey id, bool noTracking = true);
    IEnumerable<TEntity> GetAll(bool noTracking = true);
    bool Exists(TKey id);

    Task<TEntity?> FirstOrDefaultAsync(TKey id, bool noTracking = true);
    Task<IEnumerable<TEntity>> GetAllAsync(bool noTracking = true);
    Task<bool> ExistsAsync(TKey id);
    Task<int> RemoveAsync(TEntity entity);
    Task<int> RemoveAsync(TKey id);

}