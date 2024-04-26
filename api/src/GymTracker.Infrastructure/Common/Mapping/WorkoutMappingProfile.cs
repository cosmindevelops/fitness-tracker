using AutoMapper;
using GymTracker.Core.Entities;

namespace GymTracker.Infrastructure.Common.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Workout, WorkoutResponseDto>()
            .ForMember(dest => dest.Exercises, opt => opt.MapFrom(src => src.Exercises));
        CreateMap<Exercise, ExerciseResponseDto>();
        CreateMap<Series, SeriesResponseDto>();

        CreateMap<WorkoutCreateDto, Workout>();
        CreateMap<WorkoutUpdateDto, Workout>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<ExerciseCreateDto, Exercise>();
        CreateMap<ExerciseUpdateDto, Exercise>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<SeriesCreateDto, Series>();
        CreateMap<SeriesUpdateDto, Series>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<User, UserDto>()
           .ForMember(dest => dest.UserId, act => act.MapFrom(src => src.Id))
           .ForMember(dest => dest.Username, act => act.MapFrom(src => src.Username))
           .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Email));
    }
}