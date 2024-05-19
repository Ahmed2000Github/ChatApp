using AutoMapper;
using ChatAppCore.DTOs;
using ChatAppCore.Entities;
namespace ChatAppServer.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<Message, MessageDTO>();
            CreateMap<Message, MessageFormDTO>();
        }
    }
}
