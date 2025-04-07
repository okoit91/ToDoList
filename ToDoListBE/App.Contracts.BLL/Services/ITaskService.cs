using App.BLL.DTO;
using App.Contracts.DAL.Repositories;
using Base.Contracts.DAL;

namespace App.Contracts.BLL.Services;

public interface ITaskService :
    IEntityRepository<App.BLL.DTO.Task>, ITaskRepositoryCustom<App.BLL.DTO.Task>
{
    Task<IEnumerable<App.BLL.DTO.Task>> GetAllSortedAsync();
    
}