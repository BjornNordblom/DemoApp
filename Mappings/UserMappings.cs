using DemoApp.Entities;
using AutoMapper;

namespace DemoApp.Persistence;

public class DomainToResponseMappingProfile : Profile
{
    public DomainToResponseMappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<User, UserShallowDto>();
        CreateMap<Post, PostDto>();
        CreateMap<Post, PostShallowDto>();
        CreateMap<UserPost, UserPostDto>();
        CreateMap<UserPostRelation, UserPostRelationDto>();
    }
}
