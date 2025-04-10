using Base.Contracts.DAL;
using DALDTO = App.DAL.DTO;

namespace App.Contracts.DAL.Repositories;

public interface IToDoListRepository : IEntityRepository<DALDTO.ToDoList>, IToDoListRepositoryCustom<DALDTO.ToDoList>
{ 
    
}

public interface IToDoListRepositoryCustom<TEntity>
{
    
    Task<IEnumerable<TEntity>> GetAllSortedAsync();
    
    Task<TEntity?> FirstOrDefaultByNameAsync(string name);
    
    Task<IEnumerable<App.DAL.DTO.ToDoList>> GetSubListsAsync(Guid parentId);
}