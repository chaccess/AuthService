using Application.Common.Compiled;
using Application.Exceptions;
using Application.Common.Settings;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Options;
using System.Text;

namespace Application.Services.VerificationCodesService;

public class VerificationCodesService(
  IRepository repository,
  IOptions<VerificationCodesSettings> options) : IVerificationCodesService
{
    private readonly IRepository _repository = repository;
    private readonly Random _rnd = new();
    private readonly IOptions<VerificationCodesSettings> _options = options;

    public async Task<string> SendCode(string login)
    {
        bool isPhone = PreCompiledData.PhoneRegex().IsMatch(login);
        bool isEmail = PreCompiledData.EmailRegex().IsMatch(login);

        User? user;

        if (isPhone)
        {
            user = await this._repository.GetUserByPhoneAsync(login);
        }
        else if (isEmail)
        {
            user = await this._repository.GetUserByEmailAsync(login);
        }
        else
        {
            throw new BadRequestException(login + " не является телефоном или емейлом");
        }
        if (user == null)
        {
            throw new NotFoundException("Пользователь не найден");
        }

        string codeString = this.GenerateNewCode();

        VerificationCode verificationCode = new()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = codeString,
            Type = VerificationCodeType.Auth,
            Destination = isPhone ? VerificationCodeDestination.Phone : VerificationCodeDestination.Email,
            TimeToLive = _options.Value.CodeTimeToLive,
        };

        await _repository.AddVerificationCodeAsync(verificationCode);
        await _repository.Commit();

        return isPhone ? "phone" : "email";
    }

    public async Task<bool> VerifyCode(User user, string code, bool isPhone)
    {
        var lastCode = await _repository.GetUserLastSmsVerificationCodeAsync(user.Id, isPhone);

        if (!(lastCode?.Code == code) || !(lastCode.CreateDate.AddSeconds(_options.Value.CodeTimeToLive) >= DateTime.UtcNow))
        {
            return false;
        }

        lastCode.Used = true;
        await _repository.Commit();
        return true;
    }

    private string GenerateNewCode()
    {
        var stringBuilder = new StringBuilder();

        for (int i = 0; i < this._options.Value.CodeLength; ++i)
        {
            stringBuilder.Append(this._rnd.Next(0, 9));
        }

        return stringBuilder.ToString();
    }
}
