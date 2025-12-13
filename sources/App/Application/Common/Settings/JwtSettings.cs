namespace Application.Common.Settings;

public class JwtSettings
{
    public const string Key = "JWT";
    public string JwtSecretString { get; set; }
    public string LiveKitSecretString { get; set; }
}
