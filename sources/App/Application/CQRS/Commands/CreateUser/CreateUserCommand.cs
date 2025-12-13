using Domain.Entities;
using MediatR;

namespace Application.CQRS.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<User>
    {
        public string Email { get; set; }

        public string Phone { get; set; }

        public UserRole? Role { get; set; }
    }
}
