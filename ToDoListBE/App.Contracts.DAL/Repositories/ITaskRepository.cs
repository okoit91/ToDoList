using Base.Contracts.DAL;
using DALDTO = App.DAL.DTO;

namespace App.Contracts.DAL.Repositories;

public interface ITaskRepository : IEntityRepository<DALDTO.Task>, ITaskRepositoryCustom<DALDTO.Task>
{ 
}


public interface ITaskRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllSortedAsync();
    
    Task<TEntity?> FirstOrDefaultByNameAsync(string name);
}