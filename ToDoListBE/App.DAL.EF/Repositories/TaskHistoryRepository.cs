using App.Contracts.DAL.Repositories;
using AutoMapper;
using Base.DAL.EF;
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;


public class TaskHistoryRepository : BaseEntityRepository<APPDomain.TaskHistory, DALDTO.TaskHistory, AppDbContext>,
    ITaskHistoryRepository
{
    public TaskHistoryRepository(AppDbContext dbContext, IMapper mapper) :
        base(dbContext, new DalDomainMapper<APPDomain.TaskHistory, DALDTO.TaskHistory>(mapper))
    {
    }
    

    public async Task<IEnumerable<DALDTO.TaskHistory>> GetAllSortedAsync()
    {
        var query = CreateQuery();
        var res = await query.ToListAsync();
        return res.Select(e => Mapper.Map(e)).ToList();
    }

    public async Task<DALDTO.TaskHistory?> FirstOrDefaultByNameAsync(string title)
    {
        var query = CreateQuery();
        var domainCity = await query.FirstOrDefaultAsync(c => c.CurrentTitle == title);
        return domainCity == null ? null : Mapper.Map(domainCity);
    }
    
}