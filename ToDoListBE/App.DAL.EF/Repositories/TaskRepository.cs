using App.Contracts.DAL.Repositories;
using AutoMapper;
using Base.DAL.EF;
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;


public class TaskRepository :  BaseEntityRepository<APPDomain.Task, DALDTO.Task, AppDbContext>, ITaskRepository
{
    public TaskRepository(AppDbContext dbContext, IMapper mapper) :
        base(dbContext, new DalDomainMapper<APPDomain.Task, DALDTO.Task>(mapper))
    {
    }
    
    public async Task<IEnumerable<App.DAL.DTO.Task>> GetAllByToDoListIdAsync(Guid toDoListId)
    {
        var domainTasks = await CreateQuery()
            .Where(t => t.ToDoListId == toDoListId)
            .ToListAsync();

        return domainTasks.Select(t => Mapper.Map(t))!;
    }

    public async Task<IEnumerable<DALDTO.Task>> GetAllSortedAsync()
    {
        var query = CreateQuery()
            .Include(t => t.ToDoList);

        var res = await query.ToListAsync();

        return res.Select(e => Mapper.Map(e)).ToList();
    }

    public async Task<DALDTO.Task?> FirstOrDefaultByNameAsync(string title)
    {
        var query = CreateQuery();
        var domainCity = await query.FirstOrDefaultAsync(c => c.Title == title);
        return domainCity == null ? null : Mapper.Map(domainCity);
    }
    
    
    
}