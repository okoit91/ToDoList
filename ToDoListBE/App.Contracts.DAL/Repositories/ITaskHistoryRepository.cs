using Base.Contracts.DAL;
using DALDTO = App.DAL.DTO;

namespace App.Contracts.DAL.Repositories;

public interface ITaskHistoryRepository : IEntityRepository<DALDTO.TaskHistory>, ITaskHistoryRepositoryCustom<DALDTO.TaskHistory>
{ 
    Task<IEnumerable<DALDTO.TaskHistory>> GetAllWithIncludesAsync();
}

public interface ITaskHistoryRepositoryCustom<TEntity>
{
    
    Task<IEnumerable<TEntity>> GetAllSortedAsync();

    Task<IEnumerable<TEntity>> GetAllByTaskIdAsync(Guid taskId);
    
    
    Task<TEntity?> FirstOrDefaultByNameAsync(string name);
}