using Application.CQRS.Commands.CreateUser;
using Application.CQRS.Commands.UpdateUser;
using Application.Services.UserService.Models;
using AutoMapper;
using Domain.Entities;

namespace Application.Services.UserService.Mapping;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        MapCreateUserModelToCreateUserCommand();
        MapUpdateUserModelToUpdateUserCommand();
        MapCreateUserCommandToUser();
        MapUpdateUserCommandToUser();
    }

    private void MapCreateUserModelToCreateUserCommand() => CreateMap<CreateUserRequest, CreateUserCommand>();
    private void MapUpdateUserModelToUpdateUserCommand() => CreateMap<UpdateUserRequest, UpdateUserCommand>();
    private void MapCreateUserCommandToUser() => CreateMap<CreateUserCommand, User>();
    private void MapUpdateUserCommandToUser() => CreateMap<UpdateUserCommand, User>();
}
