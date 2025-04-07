using App.BLL.DTO;
using App.Contracts.BLL;
using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using AutoMapper;
using Base.BLL;
using ToDoList = App.BLL.DTO.ToDoList;

namespace App.BLL.services;

public class ToDoListService :
    BaseEntityService<App.DAL.DTO.ToDoList, App.BLL.DTO.ToDoList, IToDoListRepository>, IToDoListService
{
    public ToDoListService(IAppUnitOfWork uow, IToDoListRepository repository, IMapper mapper) :
        base(uow, repository, new BllDalMapper<App.DAL.DTO.ToDoList, App.BLL.DTO.ToDoList>(mapper))
    {
        
    }

    public async Task<IEnumerable<ToDoList>> GetAllSortedAsync()
    {
        return (await Repository.GetAllSortedAsync()).Select(e => Mapper.Map(e));
    }

    

    public async Task<ToDoList?> FirstOrDefaultByNameAsync(string name)
    {
        var list = await Repository.FirstOrDefaultByNameAsync(name);
        return list == null ? null : Mapper.Map(list);
    }
    
    
}