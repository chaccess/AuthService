using Application.Services.UserService;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.CQRS.Commands.UpdateUser
{
    public class UpdateUserCommandHandler(
        IUserService userService,
        IMapper mapper
        ) : IRequestHandler<UpdateUserCommand, User>
    {
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(request);

            return await _userService.UpdateUser(user);
        }
    }
}
