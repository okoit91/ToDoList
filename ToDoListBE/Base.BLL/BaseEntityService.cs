using Base.Contracts.BLL;
using Base.Contracts.DAL;
using Base.Contracts.Domain;

namespace Base.BLL;

public class BaseEntityService<TDalEntity,TBllEntity, TRepository> :
    BaseEntityService<TDalEntity,TBllEntity, TRepository, Guid>,
    IEntityService<TBllEntity> 
    where TBllEntity : class, IDomainEntityId
    where TDalEntity : class, IDomainEntityId<Guid> 
    where TRepository : IEntityRepository<TDalEntity, Guid>
{
    public BaseEntityService(IUnitOfWork uow, TRepository repository, IBLLMapper<TDalEntity, TBllEntity> Mapper) :
        base(uow, repository, Mapper)
    {
    }
}

public class BaseEntityService<TDalEntity, TBllEntity, TRepository, TKey> :
    IEntityService<TBllEntity, TKey>
    where TRepository : IEntityRepository<TDalEntity, TKey>
    where TKey : IEquatable<TKey> 
    where TBllEntity : class, IDomainEntityId<TKey>
    where TDalEntity : class, IDomainEntityId<TKey>
{
    protected readonly IUnitOfWork Uow;
    protected readonly TRepository Repository;
    protected readonly IBLLMapper<TDalEntity, TBllEntity> Mapper;

    public BaseEntityService(IUnitOfWork uow, TRepository repository, IBLLMapper<TDalEntity, TBllEntity> mapper)
    {
        Uow = uow;
        Repository = repository;
        Mapper = mapper;
    }

    public TBllEntity Add(TBllEntity entity)
    {
        return Mapper.Map(Repository.Add(Mapper.Map(entity)))!;
    }

    public TBllEntity Update(TBllEntity entity)
    {
        return Mapper.Map(Repository.Update(Mapper.Map(entity)))!;
    }
    
    public async Task<TBllEntity?> UpdateAsync(TBllEntity entity)
    {
        var existingEntity = await Repository.FirstOrDefaultAsync(entity.Id);
        if (existingEntity == null)
        {
            return null;
        }
        
        var updatedDalEntity = Mapper.Map(entity);  
        Repository.Update(updatedDalEntity);           
        await Uow.SaveChangesAsync();

        return Mapper.Map(updatedDalEntity);
    }

    public int Remove(TBllEntity entity)
    {
        return Repository.Remove(Mapper.Map(entity));
    }

    public int Remove(TKey id)
    {
        return Repository.Remove(id);
    }

    public TBllEntity? FirstOrDefault(TKey id, bool noTracking = true)
    {
        return Mapper.Map(Repository.FirstOrDefault(id, noTracking));
    }

    public IEnumerable<TBllEntity> GetAll(bool noTracking = true)
    {
        return Repository.GetAll(noTracking).Select(e => Mapper.Map(e));
    }

    public bool Exists(TKey id)
    {
        return Repository.Exists(id);
    }

    
    public async Task<TBllEntity?> FirstOrDefaultAsync(TKey id, bool noTracking = true)
    {
        var entity = await Repository.FirstOrDefaultAsync(id, noTracking);
        return Mapper.Map(entity);
    }

    public async Task<IEnumerable<TBllEntity>> GetAllAsync(bool noTracking = true)
    {
        return (await Repository.GetAllAsync(noTracking)).Select(e => Mapper.Map(e));
    }

    public async Task<bool> ExistsAsync(TKey id)
    {
        return await Repository.ExistsAsync(id);
    }


    public async Task<int> RemoveAsync(TBllEntity entity)
    {
        return await Task.Run(() => Remove(entity));
    }

    

    public async Task<int> RemoveAsync(TKey id)
    {
        return await Task.Run(() => Remove(id));
    }
}