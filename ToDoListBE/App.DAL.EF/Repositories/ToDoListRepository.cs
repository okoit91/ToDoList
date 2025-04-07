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
    public ToDoListRepository(AppDbContext dbContext, IMapper mapper) :
        base(dbContext, new DalDomainMapper<APPDomain.ToDoList, DALDTO.ToDoList>(mapper))
    {
    }
    

    public async Task<IEnumerable<DALDTO.ToDoList>> GetAllSortedAsync()
    {
        var query = CreateQuery();
        var res = await query.ToListAsync();
        return res.Select(e => Mapper.Map(e)).ToList();
    }

    public async Task<DALDTO.ToDoList?> FirstOrDefaultByNameAsync(string name)
    {
        var query = CreateQuery();
        var domainList = await query.FirstOrDefaultAsync(c => c.Name == name);
        return domainList == null ? null : Mapper.Map(domainList);
    }
}