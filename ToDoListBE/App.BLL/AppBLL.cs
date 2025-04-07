using App.BLL.services;
using App.Contracts.BLL;
using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.DAL.EF;
using AutoMapper;
using Base.BLL;

namespace App.BLL;

public class AppBLL : BaseBLL<AppDbContext>, IAppBLL
{
    private readonly IMapper _mapper;
    private readonly IAppUnitOfWork _uow;
    
    public AppBLL(IAppUnitOfWork uoW, IMapper mapper) : base(uoW)
    {
        _mapper = mapper;
        _uow = uoW;
    }
    
    private ITaskService? _tasks;
    private ITaskHistoryService? _taskHistories;
    private IToDoListService? _toDoLists;
    

    public ITaskService Tasks =>
        _tasks ?? new TaskService(_uow, _uow.TaskRepository, _mapper);
    
    public ITaskHistoryService TaskHistories =>
        _taskHistories ?? new TaskHistoryService(_uow, _uow.TaskHistoryRepository, _mapper);
    
    public IToDoListService ToDoLists =>
        _toDoLists ?? new ToDoListService(_uow, _uow.ToDoListRepository, _mapper);
    
    
}