using Domain.Entities;

namespace Application.Services.AuthService;

public interface IAuthService
{
    Task<AuthResponse> Authenticate(AuthRequest model);

    Task<AuthResponse> RefreshTokens(string refreshToken);

    Task<User?> AddUser(CreateUserModel model);

    Task<User?> GetUserByIdAsync(Guid id);
}
