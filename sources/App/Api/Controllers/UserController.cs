using Application.CQRS.Commands.CreateUser;
using Application.Services.UserService.Models;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController(
        IMediator mediator,
        IMapper mapper
        ) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IMapper _mapper = mapper;


        [HttpPost("create", Name = "createUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest createUser)
        {
            try
            {
                var command = _mapper.Map<CreateUserCommand>(createUser);
                var user = await _mediator.Send(command);

                if (user == null)
                {
                    return BadRequest();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

    }
}
