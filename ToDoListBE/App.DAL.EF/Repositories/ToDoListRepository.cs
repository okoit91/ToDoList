using App.Contracts.DAL.Repositories;
using AutoMapper;
using Base.DAL.EF;
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;


public class ToDoListRepository : BaseEntityRepository<APPDomain.ToDoList, DALDTO.ToDoList, AppDbContext>,
    IToDoListRepository
{
    private readonly AppDbContext _dbContext;
    
    public ToDoListRepository(AppDbContext dbContext, IMapper mapper) :
        base(dbContext, new DalDomainMapper<APPDomain.ToDoList, DALDTO.ToDoList>(mapper))
    {
        _dbContext = dbContext;
    }
    

    public async Task<IEnumerable<App.DAL.DTO.ToDoList>> GetAllSortedAsync()
    {
        var rootLists = await CreateQuery()
            .Where(l => l.ParentListId == null)
            .Include(l => l.Tasks)
            .Include(l => l.SubLists) // only one level deep
            .ToListAsync();

        foreach (var root in rootLists)
        {
            await LoadSubListsRecursive(root);
        }

        return rootLists.Select(e => Mapper.Map(e)).ToList();
    }
    
    protected override IQueryable<APPDomain.ToDoList> CreateQuery(bool noTracking = true)
    {
        return base.CreateQuery(noTracking)
            .Include(t => t.Tasks)
            .Include(t => t.SubLists); // only immediate children
    }
    
    private async Task LoadSubListsRecursive(App.Domain.ToDoList list)
    {
        if (list.SubLists == null || !list.SubLists.Any()) return;

        foreach (var sub in list.SubLists)
        {
            // Load Tasks for each sublist
            await _dbContext.Entry(sub).Collection(s => s.Tasks!).LoadAsync();
            // Load deeper sublists
            await _dbContext.Entry(sub).Collection(s => s.SubLists!).LoadAsync();

            await LoadSubListsRecursive(sub); // recursive call
        }
    }
    
    public async Task<IEnumerable<App.DAL.DTO.ToDoList>> GetSubListsAsync(Guid parentId)
    {
        var domainLists = await CreateQuery()
            .Where(l => l.ParentListId == parentId)
            .ToListAsync();

        return domainLists.Select(e => Mapper.Map(e))!;
    }
    
    
    
}