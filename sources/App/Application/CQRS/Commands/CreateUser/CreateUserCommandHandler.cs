using Application.Services.UserService;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.CQRS.Commands.CreateUser
{
    public class CreateUserCommandHandler(IUserService userService, IMapper mapper) : IRequestHandler<CreateUserCommand, User>
    {
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(request);

            return await _userService.CreateUserAsync(user);
        }
    }
}
