using Base.Contracts.DAL;
using Base.Contracts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Base.DAL.EF;


public class BaseEntityRepository<TDomainEntity, TDalEntity, TDbContext> :
    BaseEntityRepository<Guid, TDomainEntity, TDalEntity, TDbContext>, IEntityRepository<TDalEntity>
    where TDomainEntity : class, IDomainEntityId
    where TDalEntity : class, IDomainEntityId
    where TDbContext : DbContext
{
    
    public BaseEntityRepository(TDbContext dbContext, IDalMapper<TDomainEntity, TDalEntity> mapper) : base(dbContext, mapper)
    {
    }
    
}

public class BaseEntityRepository<TKey, TDomainEntity, TDalEntity, TDbContext>
    where TKey : IEquatable<TKey>
    where TDomainEntity : class, IDomainEntityId
    where TDalEntity : class, IDomainEntityId
    where TDbContext : DbContext
{

    protected readonly TDbContext repoDbContext;
    protected readonly DbSet<TDomainEntity> RepoDbSet;
    protected readonly IDalMapper<TDomainEntity, TDalEntity> Mapper;
    
    
    public BaseEntityRepository(TDbContext dbContext, IDalMapper<TDomainEntity, TDalEntity> mapper)
    {
        repoDbContext = dbContext;
        RepoDbSet = repoDbContext.Set<TDomainEntity>();
        Mapper = mapper;
    }

    protected virtual IQueryable<TDomainEntity> CreateQuery(bool noTracking = true)
    {
        var query = RepoDbSet.AsQueryable();

        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }
    
    
    public virtual TDalEntity Add(TDalEntity entity)
    {
        ConvertDateTimePropertiesToUtc(entity);
        return Mapper.Map(RepoDbSet.Add(Mapper.Map(entity)).Entity)!;
        
    }

    public virtual TDalEntity Update(TDalEntity entity)
    {
        return Mapper.Map(RepoDbSet.Update(Mapper.Map(entity)).Entity)!;
    }
    
    public virtual async Task<TDalEntity?> UpdateAsync(TDalEntity entity)
    {
        
        var existingEntity = await RepoDbSet.FindAsync(entity.Id);
        if (existingEntity == null)
        {
            return null; 
        }
        
        repoDbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
        
        try
        {
            await repoDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Error updating entity.", ex);
        }
        return Mapper.Map(existingEntity);
    }
    
    public virtual int Remove(TDalEntity entity)
    {
        return RepoDbSet
            .Where(e => e.Id.Equals(entity.Id))
            .ExecuteDelete();
    }
    
    

    public virtual int Remove(TKey id)
    {
        return RepoDbSet
            .Where(e => e.Id.Equals(id))
            .ExecuteDelete();
    }

    
    public virtual TDalEntity? FirstOrDefault(TKey id, bool noTracking = true)
    {
        return Mapper.Map(CreateQuery(noTracking).FirstOrDefault(m => m.Id.Equals(id)));
    }

    public virtual IEnumerable<TDalEntity> GetAll(bool noTracking = true)
    {
        return CreateQuery(noTracking).ToList().Select(de => Mapper.Map(de));
    }


    public virtual bool Exists(TKey id)
    {
        return CreateQuery().Any(e => e.Id.Equals(id));
    }
    
    
    public virtual async Task<TDalEntity?> FirstOrDefaultAsync(TKey id, bool noTracking = true)
    {
        return Mapper.Map(await CreateQuery(noTracking).FirstOrDefaultAsync(m => m.Id.Equals(id)));
    }

    public virtual async Task<IEnumerable<TDalEntity>> GetAllAsync(bool noTracking = true)
    {
        return (await CreateQuery(noTracking).ToListAsync())
            .Select(de => Mapper.Map(de));
    }

    public virtual async Task<bool> ExistsAsync(TKey id)
    {
        return await CreateQuery().AnyAsync(e => e.Id.Equals(id));
    }


    public virtual async Task<int> RemoveAsync(TDalEntity entity)
    {
        return await RepoDbSet
            .Where(e => e.Id.Equals(entity.Id))
            .ExecuteDeleteAsync();
    }

    public virtual async Task<int> RemoveAsync(TKey id)
    {
        return await RepoDbSet
            .Where(e => e.Id.Equals(id))
            .ExecuteDeleteAsync();
    }
    
    public void ConvertDateTimePropertiesToUtc(TDalEntity entity)
    {
        var dateTimeProperties = typeof(TDalEntity)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));

        foreach (var property in dateTimeProperties)
        {
            var dateTimeValue = (DateTime?)property.GetValue(entity);
            if (dateTimeValue != null)
            {
                property.SetValue(entity, dateTimeValue.Value.ToUniversalTime());
            }
        }
    }
    
    
}