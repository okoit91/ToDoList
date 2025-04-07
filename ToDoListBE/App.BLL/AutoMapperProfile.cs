using AutoMapper;

namespace App.BLL;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        
        CreateMap<App.DAL.DTO.Task, App.BLL.DTO.Task>().ReverseMap();
        
        CreateMap<App.DAL.DTO.TaskHistory, App.BLL.DTO.TaskHistory>().ReverseMap();
        
        CreateMap<App.DAL.DTO.ToDoList, App.BLL.DTO.ToDoList>().ReverseMap();
        
    }
    
}