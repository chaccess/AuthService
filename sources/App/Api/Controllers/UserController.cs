using Application.Auth.Services.AuthService;
using Application.Auth.Services.VerificationCodesService;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(
        IAuthService authService,
        IVerificationCodesService verificationCodesService
        ) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IVerificationCodesService _verificationCodesService = verificationCodesService;

        [HttpGet("getCode", Name = "GetCode")]
        public async Task<IActionResult> GetCode(string login)
        {
            return await _verificationCodesService.SendCodeViaSms(login) ? Ok() : Problem();
        }

        [HttpPost("verify", Name = "VerifyCode")]
        public async Task<IActionResult> VerifyCode([FromBody] AuthRequest model)
        {
            var response = await _authService.Authenticate(model);

            response = null;

            return response == null ? throw new Exception("Что-то пошло не так") : Ok(response);
        }

        [HttpPost("create", Name = "CreateUser")]
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
