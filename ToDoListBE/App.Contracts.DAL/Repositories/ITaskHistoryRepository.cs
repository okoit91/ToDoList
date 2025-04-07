using Base.Contracts.DAL;
using DALDTO = App.DAL.DTO;

namespace App.Contracts.DAL.Repositories;

public interface ITaskHistoryRepository : IEntityRepository<DALDTO.TaskHistory>, ITaskHistoryRepositoryCustom<DALDTO.TaskHistory>
{ 
    
}

public interface ITaskHistoryRepositoryCustom<TEntity>
{
    
    Task<IEnumerable<TEntity>> GetAllSortedAsync();
    
    
    Task<TEntity?> FirstOrDefaultByNameAsync(string name);
}