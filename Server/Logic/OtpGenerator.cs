using System.Security.Cryptography;

namespace DarkPatterns.OneTimePassword.Logic;

public class OtpGenerator : IOtpGenerator
{
	public string GenerateOtp()
	{
		return RandomNumberGenerator.GetString("0123456789", 6);
	}
}
