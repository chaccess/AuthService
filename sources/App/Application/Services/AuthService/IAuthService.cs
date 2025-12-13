using Application.Services.AuthService.Contracts;
using Domain.Entities;

namespace Application.Services.AuthService;

public interface IAuthService
{
    Task<AuthResponse> Authenticate(AuthRequest model);

    Task<AuthResponse> RefreshTokens(string refreshToken);

    Task<User?> GetUserByIdAsync(Guid id);
}
