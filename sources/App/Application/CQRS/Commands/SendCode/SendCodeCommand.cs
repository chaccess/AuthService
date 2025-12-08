using MediatR;

namespace Application.CQRS.Commands.SendCode
{
    public record SendCodeCommand(string Login) : IRequest<SendCodeCommandResponse>;
}
