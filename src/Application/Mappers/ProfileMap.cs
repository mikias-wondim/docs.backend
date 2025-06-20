using Application.Features.Users;
using AutoMapper;
using Domain.Users;

namespace Application.Mappers;

public class ProfileMap: Profile
{
    public ProfileMap()
    {
        CreateMap<User, UserResponse>();
    }
}
