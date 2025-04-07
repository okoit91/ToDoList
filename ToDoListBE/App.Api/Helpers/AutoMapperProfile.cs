using AutoMapper;

namespace WebApp.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<App.BLL.DTO.Task, App.DTO.v1_0.Task>().ReverseMap();
        CreateMap<App.BLL.DTO.ToDoList, App.DTO.v1_0.ToDoList>().ReverseMap();
        CreateMap<App.BLL.DTO.TaskHistory, App.DTO.v1_0.TaskHistory>().ReverseMap();
    }
}