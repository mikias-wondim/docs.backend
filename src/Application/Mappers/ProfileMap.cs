using Application.Features.ProjectMembers;
using Application.Features.Projects;
using Application.Features.Users;
using AutoMapper;
using Domain.ProjectMembers;
using Domain.Projects;
using Domain.Users;

namespace Application.Mappers;

public class ProfileMap : Profile
{
    public ProfileMap()
    {
        CreateMap<User, UserResponse>();
        CreateMap<User, UserSummaryResponse>();
        CreateMap<Project, ProjectResponse>()
            .ForMember(dest => dest.Owner, opt
                => opt.MapFrom(src => src.Owner));
        CreateMap<ProjectMember, ProjectMemberResponse>();
    }
}
