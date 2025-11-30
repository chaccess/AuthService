using Application.Common.Compiled;
using Domain.Entities;

namespace Application.Services.AuthService;

public class CreateUserModel
{
    public string Email { get; set; }

    public string Phone { get; set; }

    public UserRole? Role { get; set; }

    public bool Validate()
    {
        return PreCompiledData.EmailRegex().IsMatch(Email) & PreCompiledData.PhoneRegex().IsMatch(Phone) & Role.HasValue;
    }
}
