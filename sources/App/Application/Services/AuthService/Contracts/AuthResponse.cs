namespace Application.Services.AuthService.Contracts;

public class AuthResponse(bool isSuccess, string code)
{
    public bool IsSuccess { get; set; } = isSuccess;
    public string Code { get; set; } = code;
}
