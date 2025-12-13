using Application.Common.Compiled;
using Application.Common.Settings;
using Application.Exceptions;
using Application.Interfaces;
using Application.Services.AuthService.Contracts;
using Application.Services.VerificationCodesService;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services.AuthService
{
    public class AuthService(
        IOptions<JwtSettings> jwtSettings,
        IRepository repository,
        IVerificationCodesService verificationCodesService,
        IMapper mapper,
        IMemoryCache cache) : IAuthService
    {
        private readonly IOptions<JwtSettings> _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
        private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly IVerificationCodesService _verificationCodesService = verificationCodesService ?? throw new ArgumentNullException(nameof(verificationCodesService));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IMemoryCache _memoryCache = cache ?? throw new ArgumentNullException(nameof(cache));

        private const long TOKEN_LIFETIME_IN_SECONDS = 60 * 60 * 6;
        private const long REFERSH_TOKEN_LIFETIME_IN_SECONDS = 60 * 60 * 24 * 7;

        public async Task<AuthResponse> Authenticate(AuthRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            bool isPhone = PreCompiledData.PhoneRegex().IsMatch(request.Login);
            bool isEmail = PreCompiledData.EmailRegex().IsMatch(request.Login);

            User? user;

            if (isPhone)
            {
                user = await _repository.GetUserByPhoneAsync(request.Login);
            }
            else if (isEmail)
            {
                user = await _repository.GetUserByEmailAsync(request.Login);
            }
            else
            {
                throw new BadRequestException($"{request.Login} не является телефоном или емейлом");
            }

            if (user == null)
                ThrowUserNotFoundException();

            if (!await _verificationCodesService.VerifyCode(user, request.Code, isPhone))
                throw new BadRequestException("Неверный код");

            return await BuildAuthResponse(user, request.UserInfo);
        }

        public async Task<AuthResponse> RefreshTokens(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new BadRequestException("Пустой refresh token");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Value.JwtSecretString);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            SecurityToken validatedToken;
            try
            {
                tokenHandler.ValidateToken(refreshToken, validationParameters, out validatedToken);
            }
            catch (Exception)
            {
                throw new BadRequestException("Невалидный токен");
            }

            if (validatedToken is not JwtSecurityToken jwtToken)
            {
                throw new BadRequestException("Невалидный токен");
            }

            var idClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;

            if (!Guid.TryParse(idClaim, out var userId))
            {
                throw new BadRequestException("Невалидный токен");
            }

            var user = await _repository.GetUserByIdAsync(userId);
            if (user == null)
                ThrowUserNotFoundException();

            var userLastToken = await _repository.GetUserLastRefreshTokenAsync(userId);
            if (userLastToken == null || userLastToken.Body != refreshToken)
            {
                throw new BadRequestException("Неверный токен для обновления");
            }

            return await BuildAuthResponse(user);
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _repository.GetUserByIdAsync(id);
        }

        private async Task<AuthResponse> BuildAuthResponse(User user, AuthRequestUserInfo? info = null)
        {
            var lastRefreshToken = await _repository.GetUserLastRefreshTokenAsync(user.Id);
            if (lastRefreshToken != null)
            {
                lastRefreshToken.IsRevoked = true;
                await _repository.Commit();
            }

            var accessToken = GenerateJwtToken(user, refresh: false);
            var refreshToken = GenerateJwtToken(user, refresh: true);

            var newRefresh = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Body = refreshToken,
                UserId = user.Id,
                LifeTime = REFERSH_TOKEN_LIFETIME_IN_SECONDS
            };

            if (info is not null)
            {
                var userInfo = new UserInfo
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Browser = info.Browser,
                    OS = info.Os,
                    Locale = info.Locale,
                    TimeZone = info.TimeZone,
                    Device = info.Device,
                    IP = info.IP,
                };

                await _repository.AddUserInfoAsync(userInfo);
            }

            await _repository.AddRefreshTokenAsync(newRefresh);
            await _repository.Commit();

            if (!_memoryCache.TryGetValue(user.Id, out var _))
            {
                _memoryCache.Set(user.Id, accessToken);
            }

            var code = await _verificationCodesService.SetTokensGetterCode(user);

            return new AuthResponse(true, code);
        }

        private string GenerateJwtToken(User user, bool refresh = false, AuthRequestUserInfo? info = null)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Value.JwtSecretString);

            var claims = new List<Claim>
            {
                new("Id", user.Id.ToString())
            };

            if (!refresh)
            {
                claims.Add(new Claim("Email", user.Email));
                claims.Add(new Claim("Phone", user.Phone));
                claims.Add(new Claim("Role", user.Role.ToString()));

                if (info?.Device is not null)
                    claims.Add(new Claim("Device", info.Device));

                if (info?.Os is not null)
                    claims.Add(new Claim("OS", info.Os));

                if (info?.Locale is not null)
                    claims.Add(new Claim("Locale", info.Locale));

                if (info?.TimeZone is not null)
                    claims.Add(new Claim("TimeZone", info.TimeZone));

                if (info?.Browser is not null)
                    claims.Add(new Claim("Browser", info.Browser));

                if (info?.IP is not null)
                    claims.Add(new Claim("IP", info.IP));
            }

            var identity = new ClaimsIdentity(claims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddSeconds(refresh ? REFERSH_TOKEN_LIFETIME_IN_SECONDS : TOKEN_LIFETIME_IN_SECONDS),
                Issuer = "shokolade.ru",
                Audience = "shokolade.ru",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        private static void ThrowUserNotFoundException() =>
            throw new NotFoundException("Пользователь не найден");
    }
}
