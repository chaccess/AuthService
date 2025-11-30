using AutoMapper;
using Domain.Entities;

namespace Application.Services.AuthService.Mapping;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        MapCreateUserModelToUser();
    }

    private void MapCreateUserModelToUser() => CreateMap<CreateUserModel, User>();
}
