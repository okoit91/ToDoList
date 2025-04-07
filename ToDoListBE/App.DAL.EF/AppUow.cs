using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.EF.Repositories;
using AutoMapper;
using Base.DAL.EF;

namespace App.DAL.EF;

public class AppUow : BaseUnitOfWork<AppDbContext>, IAppUnitOfWork
{
    private readonly IMapper _mapper;
    public AppUow(AppDbContext dbContext, IMapper mapper) : base(dbContext)
    {
        _mapper = mapper;
    }
    
    private ITaskRepository? _taskRepository;
    private ITaskHistoryRepository? _taskHistoryRepository;
    private IToDoListRepository? _toDoListRepository;
    
    public ITaskRepository TaskRepository => _taskRepository ?? new TaskRepository(UowDbContext, _mapper);
    public ITaskHistoryRepository TaskHistoryRepository => _taskHistoryRepository ?? new TaskHistoryRepository(UowDbContext, _mapper);
    public IToDoListRepository ToDoListRepository => _toDoListRepository ?? new ToDoListRepository(UowDbContext, _mapper);
    
}