using App.Contracts.DAL.Repositories;
using Base.Contracts.DAL;

namespace App.Contracts.BLL.Services;

public interface ITaskHistoryService :
    IEntityRepository<App.BLL.DTO.TaskHistory>, ITaskHistoryRepositoryCustom<App.BLL.DTO.TaskHistory>
{
    
}