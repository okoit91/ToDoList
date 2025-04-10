using App.BLL.DTO;
using App.Contracts.DAL.Repositories;
using Base.Contracts.DAL;

namespace App.Contracts.BLL.Services;

public interface IToDoListService :
    IEntityRepository<App.BLL.DTO.ToDoList>, ITaskRepositoryCustom<App.BLL.DTO.ToDoList>
{
    Task<IEnumerable<App.BLL.DTO.ToDoList>> GetAllSortedAsync();
    
    public Task<IEnumerable<ToDoList>> GetSubListsAsync(Guid parentId);
    
}