using System.ComponentModel;

namespace Application.Services.AuthService;

public class AuthRequest
{
    [DefaultValue("79991115544")]
    public string EmailOrPhone { get; set; }

    [DefaultValue("000000")]
    public string Code { get; set; }
}
