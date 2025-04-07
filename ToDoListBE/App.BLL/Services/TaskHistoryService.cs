using App.BLL.DTO;
using App.Contracts.BLL;
using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using AutoMapper;
using Base.BLL;
using TaskHistory = App.BLL.DTO.TaskHistory;

namespace App.BLL.services;

public class TaskHistoryService :
    BaseEntityService<App.DAL.DTO.TaskHistory, App.BLL.DTO.TaskHistory, ITaskHistoryRepository>, ITaskHistoryService
{
    public TaskHistoryService(IAppUnitOfWork uow, ITaskHistoryRepository repository, IMapper mapper) :
        base(uow, repository, new BllDalMapper<App.DAL.DTO.TaskHistory, App.BLL.DTO.TaskHistory>(mapper))
    {
        
    }


    public async Task<IEnumerable<TaskHistory>> GetAllSortedAsync()
    {
        return (await Repository.GetAllSortedAsync()).Select(e => Mapper.Map(e));
    }
    

    public async Task<TaskHistory?> FirstOrDefaultByNameAsync(string title)
    {
        var task = await Repository.FirstOrDefaultByNameAsync(title);
        return task == null ? null : Mapper.Map(task);
    }
    
    
}