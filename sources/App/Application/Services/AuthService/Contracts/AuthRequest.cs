using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Services.AuthService.Contracts;

public class AuthRequest
{
    [DefaultValue("79991115544")]
    [Required]
    public string Login { get; set; }

    [DefaultValue("000000")]
    [Required]
    public string Code { get; set; }

    public AuthRequestUserInfo? UserInfo { get; set; }
}

public class AuthRequestUserInfo
{
    public string? Browser { get; set; }
    public string? Os { get; set; }
    public string? Device { get; set; }
    public string? Locale { get; set; }
    public string? TimeZone { get; set; }
    public string? IP { get; set; }
}