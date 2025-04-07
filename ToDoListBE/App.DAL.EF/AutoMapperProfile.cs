using AutoMapper;

namespace App.DAL.EF;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    
    {
        CreateMap<App.Domain.Task, App.DAL.DTO.Task>().ReverseMap();
        CreateMap<App.Domain.TaskHistory, App.DAL.DTO.TaskHistory>().ReverseMap();
        CreateMap<App.Domain.ToDoList, App.DAL.DTO.ToDoList>().ReverseMap();
    }
    
}