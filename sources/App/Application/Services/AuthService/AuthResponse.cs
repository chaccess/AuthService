namespace Application.Services.AuthService;

public class AuthResponse(string token, string refreshToken, long tokenlifetime, long refreshTokenlifetime)
{
    public string Token { get; set; } = token;

    public string RefreshToken { get; set; } = refreshToken;

    public long TokenLifeTime { get; set; } = tokenlifetime;

    public long RefreshTokenLifeTime { get; set; } = refreshTokenlifetime;
}
