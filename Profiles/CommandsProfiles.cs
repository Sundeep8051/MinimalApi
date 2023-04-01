using AutoMapper;
using SixMinApi.Dtos;
using SixMinApi.Models;

namespace SixMinAPI.Profiles;

public class CommandsProfile : Profile
{
    public CommandsProfile()
    {
        CreateMap<Command, CommandReadDto>();
        CreateMap<CommandCreateDto, Command>();
        CreateMap<CommandUpdateDto, Command>();
    }
}