using App.BLL.DTO;
using App.Contracts.DAL.Repositories;
using Base.Contracts.DAL;

namespace App.Contracts.BLL.Services;

public interface IToDoListService :
    IEntityRepository<App.BLL.DTO.ToDoList>, IToDoListRepositoryCustom<App.BLL.DTO.ToDoList>
{
    Task<IEnumerable<App.BLL.DTO.ToDoList>> GetAllSortedAsync();
    
    public System.Threading.Tasks.Task DeleteListAndRelatedDataAsync(Guid listId);
    
}