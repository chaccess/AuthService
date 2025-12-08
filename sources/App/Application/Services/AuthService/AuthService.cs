using Application.Common.Compiled;
using Application.Common.Settings;
using Application.Exceptions;
using Application.Interfaces;
using Application.Services.VerificationCodesService;
using AutoMapper;
using Domain.Entities;
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
        IMapper mapper) : IAuthService
    {
        private readonly IOptions<JwtSettings> _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
        private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly IVerificationCodesService _verificationCodesService = verificationCodesService ?? throw new ArgumentNullException(nameof(verificationCodesService));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        private const long TOKEN_LIFETIME_IN_SECONDS = 60 * 60 * 6;
        private const long REFERSH_TOKEN_LIFETIME_IN_SECONDS = 60 * 60 * 24 * 7;

        public async Task<AuthResponse> Authenticate(AuthRequest model)
        {
            ArgumentNullException.ThrowIfNull(model);

            bool isPhone = PreCompiledData.PhoneRegex().IsMatch(model.Login);
            bool isEmail = PreCompiledData.EmailRegex().IsMatch(model.Login);

            User? user;

            if (isPhone)
            {
                user = await _repository.GetUserByPhoneAsync(model.Login);
            }
            else if (isEmail)
            {
                user = await _repository.GetUserByEmailAsync(model.Login);
            }
            else
            {
                throw new BadRequestException($"{model.Login} не является телефоном или емейлом");
            }

            if (user == null)
                ThrowUserNotFoundException();

            if (!await _verificationCodesService.VerifyCode(user, model.Code, isPhone))
                throw new BadRequestException("Неверный код");

            return await BuildAuthResponse(user);
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
            catch (Exception ex)
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

        public async Task<User?> AddUser(CreateUserModel userModel)
        {
            ArgumentNullException.ThrowIfNull(userModel);

            var user = _mapper.Map<User>(userModel);
            user.Id = Guid.NewGuid();

            if (await UserExists(user))
            {
                throw new AlreadyExistsException("Пользователь уже существует");
            }

            await _repository.AddUserAsync(user);
            await _repository.Commit();

            return user;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _repository.GetUserByIdAsync(id);
        }

        private async Task<bool> UserExists(User user)
        {
            var byId = await _repository.UserExistsByIdAsync(user.Id);
            var byEmail = await _repository.UserExistsByEmailAsync(user.Email);
            var byPhone = await _repository.UserExistsByPhoneAsync(user.Phone);

            return byId || byEmail || byPhone;
        }

        private async Task<AuthResponse> BuildAuthResponse(User user)
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

            await _repository.AddRefreshTokenAsync(newRefresh);
            await _repository.Commit();

            return new AuthResponse(accessToken, refreshToken, TOKEN_LIFETIME_IN_SECONDS, REFERSH_TOKEN_LIFETIME_IN_SECONDS);
        }

        private string GenerateJwtToken(User user, bool refresh = false)
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
            }

            var identity = new ClaimsIdentity(claims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddSeconds(refresh ? REFERSH_TOKEN_LIFETIME_IN_SECONDS : TOKEN_LIFETIME_IN_SECONDS),
                Issuer = "auth.shokolad.ru",
                Audience = "shokolad.ru",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        private static void ThrowUserNotFoundException() =>
            throw new NotFoundException("Пользователь не найден");
    }
}
