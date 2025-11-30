using Domain.Entities;

namespace Application.Services.VerificationCodesService;

public interface IVerificationCodesService
{
    Task<bool> SendCodeViaSms(string login);

    Task<bool> VerifySmsCode(User user, string code);
}
