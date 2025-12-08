using Api.ViewModels;
using Application.CQRS.Commands.SendCode;
using Application.Services.AuthService;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        IAuthService authService,
        IMediator mediator
        ) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IMediator _mediator = mediator;

        [HttpPost("sendCode", Name = "sendCode")]
        public async Task<IActionResult> SendCode([FromBody] SendCodeRequest request)
        {
            var res = await _mediator.Send(new SendCodeCommand(request.Login));

            return res.IsSuccess ? Ok(res) : Problem();
        }

        [HttpPost("verify", Name = "verifyCode")]
        public async Task<IActionResult> VerifyCode([FromBody] AuthRequest model)
        {
            var response = await _authService.Authenticate(model);

            return response == null ? throw new Exception("Что-то пошло не так") : Ok(response);
        }

        [HttpPost("create", Name = "createUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel createUser)
        {
            if (!createUser.Validate())
            {
                return BadRequest();
            }

            try
            {
                var user = await _authService.AddUser(createUser);

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

        [HttpGet("refresh", Name = "RefreshTokens")]
        public async Task<IActionResult> RefreshTokens(string token)
        {
            try
            {
                var res = await _authService.RefreshTokens(token);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
