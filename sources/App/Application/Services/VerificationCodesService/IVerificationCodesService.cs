using Domain.Entities;

namespace Application.Services.VerificationCodesService;

public interface IVerificationCodesService
{
    Task<string> SendCode(string login);

    Task<bool> VerifyCode(User user, string code, bool isPhone);
}
