using Domain.Entities;

namespace Application.Services.VerificationCodesService;

public interface IVerificationCodesService
{
    Task<(bool IsSuccess, string LoginType)> SendCode(string login);
    Task<bool> VerifyCode(User user, string code, bool isPhone);
    Task<string> SetTokensGetterCode(User user);
}
