using Application.Features.Projects;
using Application.Features.Users;
using AutoMapper;
using Domain.Projects;
using Domain.Users;

namespace Application.Mappers;

public class ProfileMap: Profile
{
    public ProfileMap()
    {
        CreateMap<User, UserResponse>();
        CreateMap<Project, ProjectResponse>();
    }
}
