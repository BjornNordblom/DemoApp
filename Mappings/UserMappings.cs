using DemoApp.Entities;
using AutoMapper;

namespace DemoApp.Persistence;

public class DomainToResponseMappingProfile : Profile
{
    public DomainToResponseMappingProfile()
    {
        // CreateMap<string, Guid>().ConvertUsing(s => Guid.Parse(s));
        // CreateMap<string, Guid?>()
        //     .ConvertUsing(s => String.IsNullOrWhiteSpace(s) ? (Guid?)null : Guid.Parse(s));
        // CreateMap<Guid?, string>().ConvertUsing(g => g.ToString() ?? "");
        // CreateMap<Guid, string>().ConvertUsing(g => g.ToString("N"));

        CreateMap<User, UserDto>();
        CreateMap<User, UserShallowDto>();
        CreateMap<Post, PostDto>();
        CreateMap<Post, PostShallowDto>();
        CreateMap<UserPost, UserPostDto>();
        CreateMap<UserPostRelation, UserPostRelationDto>();
    }
}
