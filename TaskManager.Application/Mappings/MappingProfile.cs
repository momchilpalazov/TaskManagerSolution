
using AutoMapper;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateTaskDto, TaskItem>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<TaskManager.Domain.Entities.TaskStatus>(src.Status)))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => Enum.Parse<TaskManager.Domain.Entities.TaskPriority>(src.Priority)));

            CreateMap<TaskItem, TaskDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.AssignedToEmail, opt => opt.MapFrom(src => src.AssignedToUser.Email));

            CreateMap<Label, LabelDto>();
            CreateMap<TaskItem, TaskDto>()
                .ForMember(dest => dest.Labels, opt => opt.MapFrom(src => src.TaskLabels.Select(tl => tl.Label)));

        }
    }
}
