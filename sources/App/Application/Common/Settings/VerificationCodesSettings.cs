namespace Application.Common.Settings;

public class VerificationCodesSettings
{
	public const string Key = "VerificationCodes";

	public int UserAuthCodeLength { get; set; }
	public int UserAuthCodeTimeToLive { get; set; }

	public int GetTokenCodeLength { get; set; }
	public int GetTokenCodeTimeToLive { get; set; }
}
