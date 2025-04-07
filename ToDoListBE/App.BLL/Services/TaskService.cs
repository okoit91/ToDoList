using App.BLL.DTO;
using App.Contracts.BLL;
using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using AutoMapper;
using Base.BLL;
using Task = App.BLL.DTO.Task;

namespace App.BLL.services;

public class TaskService :
    BaseEntityService<App.DAL.DTO.Task, App.BLL.DTO.Task, ITaskRepository>, ITaskService
{
    public TaskService(IAppUnitOfWork uow, ITaskRepository repository, IMapper mapper) :
        base(uow, repository, new BllDalMapper<App.DAL.DTO.Task, App.BLL.DTO.Task>(mapper))
    {
        
    }
    public async Task<IEnumerable<Task>> GetAllSortedAsync()
    {
        return (await Repository.GetAllSortedAsync()).Select(e => Mapper.Map(e));
    }
    
    public async Task<Task?> FirstOrDefaultByNameAsync(string title)
    {
        var task = await Repository.FirstOrDefaultByNameAsync(title);
        return task == null ? null : Mapper.Map(task);
    }
    
}