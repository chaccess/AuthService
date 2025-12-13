using Api.ViewModels;
using Application.Services.AuthService;
using Application.Services.AuthService.Contracts;
using Application.Services.VerificationCodesService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        IAuthService authService,
        IVerificationCodesService codesService
        ) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IVerificationCodesService _codesService = codesService;

        [HttpPost("sendCode", Name = "sendCode")]
        [Authorize(Policy = "OnlyAnonymous")]
        public async Task<IActionResult> SendCode([FromBody] SendCodeRequest request)
        {
            var res = await _codesService.SendCode(request.Login);

            return res.IsSuccess ? Ok(res) : Problem();
        }

        [HttpPost("verify", Name = "verifyCode")]
        [Authorize(Policy = "OnlyAnonymous")]
        public async Task<IActionResult> VerifyCode([FromBody] AuthRequest model)
        {
            if (model.UserInfo is not null)
            {
                model.UserInfo.IP = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "no-data";
            }

            var response = await _authService.Authenticate(model);

            return response == null ? throw new Exception("Что-то пошло не так") : Ok(response);
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
