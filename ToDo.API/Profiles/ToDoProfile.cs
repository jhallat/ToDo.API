using AutoMapper;

namespace ToDo.API.Profiles
{
    public class ToDoProfile : Profile
    {
        public ToDoProfile()
        {
            CreateMap<Entities.ToDo, Models.ToDoDto>();
            CreateMap<Models.ToDoCreationDto, Entities.ToDo>();
        }
    }
}