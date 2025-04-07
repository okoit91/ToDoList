using App.Contracts.DAL.Repositories;
using Base.Contracts.DAL;

namespace App.Contracts.DAL;

public interface IAppUnitOfWork : IUnitOfWork
{
    ITaskRepository TaskRepository { get; }
    ITaskHistoryRepository TaskHistoryRepository { get; }
    IToDoListRepository ToDoListRepository { get; }
}