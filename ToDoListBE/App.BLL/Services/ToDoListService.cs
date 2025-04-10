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
    
    private readonly IMapper _autoMapper;
    public ToDoListService(
        IAppUnitOfWork uow,
        IToDoListRepository repository,
        IMapper mapper
    ) : base(uow, repository, new BllDalMapper<App.DAL.DTO.ToDoList, App.BLL.DTO.ToDoList>(mapper))
    {
        _autoMapper = mapper;
    }
    

    public async Task<IEnumerable<ToDoList>> GetAllSortedAsync()
    {
        var rawLists = await Repository.GetAllSortedAsync();
        return rawLists.Select(MapWithChildren).ToList();
    }

    public async Task<IEnumerable<App.BLL.DTO.ToDoList>> GetSubListsAsync(Guid parentListId)
    {
        var allLists = await Repository.GetAllAsync();
        return allLists
            .Where(l => l.ParentListId == parentListId)
            .Select(l => _autoMapper.Map<App.DAL.DTO.ToDoList, App.BLL.DTO.ToDoList>(l))
            .ToList();
    }

    public Task<ToDoList?> FirstOrDefaultByNameAsync(string name)
    {
        throw new NotImplementedException();
    }


    private ToDoList MapWithChildren(App.DAL.DTO.ToDoList entity)
    {
        var mapped = _autoMapper.Map<ToDoList>(entity);

        if (entity.SubLists != null && entity.SubLists.Any())
        {
            mapped.SubLists = entity.SubLists.Select(MapWithChildren).ToList();
        }

        if (entity.Tasks != null && entity.Tasks.Any())
        {
            mapped.Tasks = entity.Tasks
                .Select(t => _autoMapper.Map<DTO.Task>(t))
                .ToList();
        }

        return mapped;
    }
    
}