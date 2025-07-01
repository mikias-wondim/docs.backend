using Application.Features.Invitations;
using Application.Features.ProjectMembers;
using Application.Features.Projects;
using Application.Features.Sections;
using Application.Features.Users;
using AutoMapper;
using Domain.Invitations;
using Domain.ProjectMembers;
using Domain.Projects;
using Domain.Sections;
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
        CreateMap<Project, ProjectSummaryResponse>();
        CreateMap<ProjectMember, ProjectMemberResponse>();
        CreateMap<Invitation, InvitationResponse>();
        CreateMap<Section, SectionResponse>();
        CreateMap<SectionUserAccess, SectionUserAccessResponse>();
    }
}
